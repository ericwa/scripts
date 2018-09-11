open System.IO
open System.Text.RegularExpressions
let systemRegex = new Regex("""\s*\#include\s*<([^"]*)>\s*""")

let readStdin = seq {
    let stdin = System.Console.In
    let mutable keepLooping = true

    while keepLooping do
        let line = System.Console.In.ReadLine()
        if isNull line then keepLooping <- false
        else yield line
}

let sysIncludeWantsIfdef (line:string) =
    if line.StartsWith("wx/") then
        true
    else if line.EndsWith(".h") then
        false
    else
        true

let fullLineWantsIfdef (line:string) =
    let sys = systemRegex.Match(line)
    if sys.Success then
        let sysVal = sys.Groups.[1].Value
        sysIncludeWantsIfdef sysVal
    else
        false


let visitFile (filename:string) =
    printfn "visiting %s" filename
    let newLinesOfFile =  File.ReadAllLines(filename) |> Seq.filter (fun l -> not (fullLineWantsIfdef l))
    use out = new StreamWriter(filename)
    newLinesOfFile |> Seq.iter (fun l -> out.WriteLine(l))

let visitDir (dir:string) =
    let cppFiles = Directory.EnumerateFiles(dir, "*.cpp", SearchOption.AllDirectories)
    let hFiles = Directory.EnumerateFiles(dir, "*.h", SearchOption.AllDirectories)
    cppFiles |> Seq.iter visitFile
    hFiles |> Seq.iter visitFile

let () =
    visitDir "/Users/ericwa/dev/TrenchBroomAlt/common"