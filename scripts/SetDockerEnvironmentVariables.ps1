foreach($line in Get-Content .\Devi.ServiceHosts.WebApi\Docker.env) 
{
   if ($line.Length -gt 0)
   {
	   $tmp = $line.Split("=")
     
	   [System.Environment]::SetEnvironmentVariable($tmp[0], $tmp[1])
   }
}