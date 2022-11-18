#ProjectName = 1
#Location = 2

echo "Creating service file for $1 project...";

# Create or overwrite the service file.
echo "
[Unit]
Description=$1 Web Application
[Service]
WorkingDirectory=/home/admin/dotnet/applications/$1
ExecStart=/usr/bin/dotnet $2
Restart=always
RestartSec=10
SyslogIdentifier=netcore-$1
User=root
Environment=DOTNET_ENVIRONMENT=Production
[Install]
WantedBy=multi-user.target" > "/etc/systemd/system/$1.service"

chmod 0755 "/home/admin/dotnet/applications/$1"
systemctl daemon-reload
service nginx restart
systemctl restart "$1.service"