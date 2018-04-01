open System
open System.IO
open System.Collections.Generic

// F-Sharp script for auto-fixing gcc7 -Wsuggest-override warnings in a codebase,
// given a transcript of gcc's warning output.
// WARNING: Modifies source files in place!
// Run on a clean working copy so you can revert / review!
//
// This is very hacky / fragile... check for "// FIXME: add override" which means it couldn't figure out where to insert the "override" on a line.
//
// usage: 'fsharpi fix.fsx path_to_gcc_output.txt'

let isOverrideWarning (s : string) =
    s.Contains("can be marked override [-Wsuggest-override]") || 
    s.Contains("overrides a destructor but is not marked 'override' [-Winconsistent-missing-destructor-override]")

type Fix = {
    file : string
    line : int
}
let parseFix (s : string ) =
    match (isOverrideWarning s) with 
    | false -> 
        //printfn "parsed None"
        None
    | true ->
        let firstColon = s.IndexOf(":")
        let secondColon = s.IndexOf(":", firstColon + 1)
        let line = s.Substring(firstColon + 1, secondColon - (firstColon + 1))
        let file = s.Substring(0, firstColon)
        //printfn "file: '%s' line: '%s'" file line
        Some {file = file
              line = (int line) - 1}

let getLinesOfFile filename = seq {
    use f = File.OpenText(filename)
    while not f.EndOfStream do
       let line = f.ReadLine()
       yield line
}

let writeLinesOfFile filename (lines : array<string>) =
    System.IO.File.WriteAllLines(filename, lines)

let fixLine (l : string) =
    let tr = l.Trim()
    match tr.Length with
    | 0 ->
        l + "// FIXME: add override"
    | _ ->
        if tr.EndsWith(";") then
            l.Insert(l.LastIndexOf(';'), " override")
        else if tr.EndsWith("{") then
            l.Insert(l.LastIndexOf('{'), "override ")
        else if tr.EndsWith("{}") then
            l.Insert(l.LastIndexOf('{'), "override ")
        else if tr.Contains("{") then
            l.Insert(l.IndexOf('{'), "override ")
        else
            l + "// FIXME: add override"

let applyFix (f : Fix) =
    printfn "fix: %A" f
    let sourceLines = (getLinesOfFile f.file) |> Seq.toArray
    let line = sourceLines.[f.line]
    let fixedline = fixLine line
    printfn "before %A\n after: %A" line fixedline
    sourceLines.[f.line] <- fixedline
    writeLinesOfFile f.file sourceLines
    //printfn "before %s\n after: %s" line fixedline

let () =
    let args = fsi.CommandLineArgs
    match args.Length with
    | 2 ->
        let lineSeq = getLinesOfFile args.[1]
        let fixSet = new HashSet<Fix>()

        // add the fixes to a set to de-duplicate them
        lineSeq
        |> Seq.map parseFix
        |> Seq.iter (Option.iter (fun fix ->
            fixSet.Add fix |> ignore
            ))

        // apply them
        fixSet
        |> Seq.iter applyFix
    | _ -> 
        printfn "usage: 'fsharpi fix.fsx path_to_gcc_output.txt'"
