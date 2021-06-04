# DNS Exfiltration

## DNS Server Bind
The server I used for this project is DNS Server Bind
```
sudo apt-get install -y bind9
```
## Config
### Log file
Create file for storing log
```
sudo mkdir /var/log/named/qrlog
```
### /etc/bind/named.conf
```
include "/etc/bind/named.conf.options";
include "/etc/bind/named.conf.local";
include "/etc/bind/named.conf.default-zones";

logging{
	channel querylog{
		file "/var/log/named/qrlog";
		print-category yes;
		print-time yes;
	};
	category queries{ "querylog";};
};
```
### /etc/bind/named.conf.local
```
zone "example.com" IN {
	type master;
	file "/etc/bind/forward.example.com";	
};

zone "110.168.192.in-addr-arpa" IN {
	type master;
	file "/etc/bind/reverse.example.com";
};
```
### /etc/bind/forward.example.com
My address DNS Server is `192.168.110.154`
```
;
; BIND data file for local loopback interface
;
$TTL	604800
@	IN	SOA	server.example.com. root.server.example.com. (
			      2		; Serial
			 604800		; Refresh
			  86400		; Retry
			2419200		; Expire
			 604800 )	; Negative Cache TTL
;
@	IN	NS	server.example.com.
@	IN	A	192.168.110.153
server	IN	A	192.168.110.153
host 	IN	A	192.168.110.153
```
### /etc/bind/reverse.example.com
```
;
; BIND data file for local loopback interface
;
$TTL	604800
@	IN	SOA	server.example.com. root.server.example.com. (
			      2		; Serial
			 604800		; Refresh
			  86400		; Retry
			2419200		; Expire
			 604800 )	; Negative Cache TTL
;
@	IN	NS	server.example.com.
@	IN	PTR	example.com.
server	IN	A	192.168.110.153
host 	IN	A	192.168.110.153
153	IN	PTR	server.example.com.
```
### /etc/resolv.conf
```
nameserver 192.168.110.153
search example.com
```
## Validation and run bind
### Validation
> You need to check it out on google or something :D do it for yourself
### Run bind
```
# If firewall is turn on, you must allow bind9
sudo status ufw
# Enable bind9
sudo systemctl enable bind9
# Restart bind9
sudo systemctl restart bind9
```
## Extracting and decoding
### Extracting data
```
sudo egrep -o "[a-zA-Z0-9+/]+={0,2}[a-zA-Z0-9+/]+={0,2}.example.com" /var/log/named/qrlog | cut -d . -f1 | uniq | awk '!a[$0]++' > ~/Desktop/data
```
### Base64 decoding
```
sudo base64 -d ~/Desktop/data > ~/Desktop/decoded.txt
```
