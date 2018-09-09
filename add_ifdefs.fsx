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

let () =
    let mutable inIfdef = false

    readStdin |> Seq.iter (fun x ->
        let nowWants = fullLineWantsIfdef x

        if (not inIfdef) && nowWants then
            printfn "#ifndef TB_USE_PCH"
            inIfdef <- true

        if inIfdef && (not nowWants) then
            printfn "#endif"
            inIfdef <- false

        printfn "%s" x
    )