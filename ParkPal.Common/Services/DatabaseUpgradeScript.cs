using System.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using ParkPal.Common.Models.Database;

namespace ParkPal.Common.Services {

    public class DatabaseUpgradeService {
        private readonly string _databaseConnectionString;
        private readonly string _scriptsLocation;
        private DatabaseVersion? _currentVersion;
        private SqlConnection _sqlConnection;
        public DatabaseVersion _targetVersion;


        public DatabaseUpgradeService(string databaseConnectionString, string scriptsLocation) {
            _databaseConnectionString = databaseConnectionString;
            _scriptsLocation = scriptsLocation;
            _targetVersion = new DatabaseVersion(1, 0, 4);
            _sqlConnection = new SqlConnection(_databaseConnectionString);
        }

        public void UpgradeDatabase()
        {
            try
            {
                Debug.WriteLine("Connecting to SQL Server...");
                
                _currentVersion = GetCurrentDatabaseVersion();
            }
            catch (Exception ex)
            {
                throw ex;
            }finally
            {
                UpdateDatabase(_sqlConnection);
            }
        }

        public DatabaseVersion VersionFromNumber(string[] fileNumber)
        {
            return new DatabaseVersion(
                major: Convert.ToInt32(fileNumber[0]),
                minor: Convert.ToInt32(fileNumber[1]),
                revision: Convert.ToInt32(fileNumber[2])
            );
        }

        public void UpdateDatabase(SqlConnection conn)
        {
            // We will now run through all the database scripts from our 'Scripts' folder and run all the MySQL.
            DirectoryInfo ScriptsFolder = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _scriptsLocation));
            List<FileInfo> Scripts = ScriptsFolder.GetFiles().ToList();
            Scripts.Sort(new CompareFileInfoEntries());

            foreach (FileInfo fileInfo in Scripts)
            {
                string fileName = fileInfo.Name;
                string[] fileNameVersionNumbers = fileName.Replace("UpgradeScript_", "").Replace(".dbu", "").Split('.');
                DatabaseVersion scriptDatabaseVersion = VersionFromNumber(fileNameVersionNumbers);

                // If our current file database version is less than our current version, we will check
                if (_currentVersion?.Concat() < _targetVersion.Concat())
                {
                    string content = File.ReadAllText(fileInfo.FullName);

                    if (scriptDatabaseVersion.Concat() > _currentVersion.Concat() && scriptDatabaseVersion.Concat() <= _targetVersion.Concat())
                    {
                        bool databaseScriptSuccessfullyRan = RunDatabaseScript(content, conn);

                        if(databaseScriptSuccessfullyRan) {
                            AddVersionNumberToDatabase(scriptDatabaseVersion, conn);
                        }

                    }
                }

                
            }
        }

        public bool RunDatabaseScript(string content, SqlConnection conn)
        {
            try {
                string[] commands = SplitContentIntoBatches(content);

                // Loop through each command.
                foreach(string command in commands) {
                    // If the command is not an empty string, execute it.
                    if(!String.IsNullOrEmpty(command)) {

                        SqlCommand dbScriptCommand = new SqlCommand(command, conn);
                        SqlDataReader dbScriptReader = dbScriptCommand.ExecuteReader();
                        dbScriptReader.Close();

                    }
                }

                return true;

            }
            catch (Exception ex) {
                Debug.WriteLine(ex.ToString());
            }
            
            return false;
        }

        public void AddVersionNumberToDatabase(DatabaseVersion dbVersion, SqlConnection conn)
        {
            // Now we need to save each version file against the database.
            using (SqlCommand dbAddVersionCommand = new SqlCommand("AddVersionNumber", conn))
            {
                dbAddVersionCommand.CommandType = CommandType.StoredProcedure;

                dbAddVersionCommand.Parameters.AddWithValue("@Major", dbVersion.Major);
                dbAddVersionCommand.Parameters.AddWithValue("@Minor", dbVersion.Minor);
                dbAddVersionCommand.Parameters.AddWithValue("@Revision", dbVersion.Revision);

                dbAddVersionCommand.ExecuteNonQuery();

            }
        }

        public string[] SplitContentIntoBatches(string sqlString) {
            Regex _sqlScriptSplitRegEx = new Regex( @"^\s*GO\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled );

            return _sqlScriptSplitRegEx.Split(sqlString);
        }
        
        public DatabaseVersion? GetCurrentDatabaseVersion()
        {
            _sqlConnection.Open();

            string sql = "SELECT TOP 1 [Major], [Minor], [Revision] FROM [Version] ORDER BY [VersionID] DESC;";
            SqlCommand cmd = new SqlCommand(sql, _sqlConnection);
            SqlDataReader rdr = cmd.ExecuteReader();

            // If we have a version returned, we will set the current version to the database stored version.
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    // Find out the current version and run DBU scripts to bring the database up to date.
                    return new DatabaseVersion(Convert.ToInt32(rdr[0]), Convert.ToInt32(rdr[1]), Convert.ToInt32(rdr[2]));
                }
            }
            else
            {
                return new DatabaseVersion(0, 0, 0);
            }

            rdr.Close();
            
            return null;
        }
        
    }

    public class CompareFileInfoEntries : IComparer<FileInfo>
    {
        public int Compare(FileInfo? f1, FileInfo? f2)
        {
            return string.Compare(f1?.Name, f2?.Name);
        }
    }
}