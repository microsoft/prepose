
$files = get-childitem . *.smt2|where-object {!($_.psiscontainer)}
$z3path = "c:\users\dmolnar\Documents\z3bin\bin\z3.exe -st -T:3000 -t:30000"
foreach ($file in $files)
{
    $cmd = "$z3path $file"
    Invoke-Expression $cmd
}