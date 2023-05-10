foreach($line in Get-Content .\Devi.ServiceHosts.Discord\Docker.env) 
{
	$tmp = $line.Split("=")

	[System.Environment]::SetEnvironmentVariable($tmp[0], $tmp[1])
}