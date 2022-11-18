#ProjectName = 1
#Location = 2
#Port = 3
#Domain = 4


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
Environment=DOTNET_URLS=https://127.0.0.1:$3
[Install]
WantedBy=multi-user.target" > "/etc/systemd/system/$1.service"

# Now update the nginx setting that HestiaCP uses.
echo "server {
   listen      85.159.212.149:80;
   server_name $4 ;
   
   include /home/admin/conf/web/$4/nginx.forcessl.conf*;

   location / {
       proxy_pass      https://127.0.0.1:$3;
   }
   
   location @fallback {
      proxy_pass       https://127.0.0.1:$3;
   }
   
}" > "/home/admin/conf/web/$4/nginx.conf"

chmod 0755 "/home/admin/dotnet/applications/$1"
systemctl daemon-reload
service nginx restart
systemctl restart "$1.service"