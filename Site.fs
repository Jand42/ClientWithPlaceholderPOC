module ClientWithPlaceholderPOC.Site

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Server
open WebSharper.UI.Client
open WebSharper.UI.Templating

type MainTemplate = Template<"main.html", serverLoad=ServerLoad.WhenChanged>

[<JavaScript>]
module Client =

    let onum, num, op = ref 0., ref 0., ref None

    let mutable display = input [attr.``type`` "text"; attr.value "0"] []

    let updateDisplay () =
        let display = display :?> Elt
        display.Value <- string num

    let D n =
        num := 10. * !num + n
        updateDisplay ()

    let C () =
        num := 0.
        updateDisplay()

    let AC () =
        num  := 0.
        onum := 0.
        op   := None
        updateDisplay ()

    let N () =
        num := - !num
        updateDisplay ()

    let E () =
        match !op with
        | None ->
            ()
        | Some f ->
            num := f !onum !num
            op  := None
            updateDisplay ()

    let O o () =
        match !op with
        | None ->
            ()
        | Some f ->
            num := f !onum !num
            updateDisplay ()
        onum := !num
        num  := 0.
        op   := Some o

    let btn caption action =
        button [if IsClient then on.click (fun _ _ -> action ())] [text caption]

    let digit (n:float) =
        btn (string n) (fun () -> D n)

    let calculator () =
        div [] [
            display
            br [] []
            div [] [
                digit 7.; digit 8.; digit 9.; btn "/" (O ( / ))
                br [] []
                digit 4.; digit 5.; digit 6.; btn "*" (O ( * ))
                br [] []
                digit 1.; digit 2.; digit 3.; btn "-" (O ( - ))
                br [] []
                digit 0.; btn "C" C; btn "AC" AC; btn "+" (O ( + ));
                br [] []
                btn "+/-" N; btn "=" E
            ]
        ]

[<Website>]
let Main =
    Application.SinglePage (fun (ctx: Context<SPA.EndPoint>) ->
        Content.Page(
            MainTemplate()
                .Main(ClientDoc.withPlaceholder (Client.calculator()))
                .Doc()
        )
    )