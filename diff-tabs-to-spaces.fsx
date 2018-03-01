open System
open System.IO

let before = "\t"
let after = "    "
let str_to_list (s : string) = [for ch in s -> ch]

let list_to_str (l : char list) = System.String(List.toArray l)

let fixupInsertion (l : string) : string =
    l.Replace(before, after)
    // let i = l.IndexOf(before)
    // if i = 0 then after + l.Substring(i+1) else l

let fixupDiffLine (l : string) : string =
    let chars = List.ofSeq l
    match chars with
        | '+' :: rest -> "+" + fixupInsertion (list_to_str rest) 
        | _ -> l  // just return the line

let getLinesOfFile () = seq {
    use f = new StreamReader(Console.OpenStandardInput())
    while not f.EndOfStream do
       let line = f.ReadLine()
       yield line
}

let () =
    use out = new StreamWriter(Console.OpenStandardOutput())
    getLinesOfFile ()
        |> Seq.map fixupDiffLine
        |> Seq.iter out.WriteLine
