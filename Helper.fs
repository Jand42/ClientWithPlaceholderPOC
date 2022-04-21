namespace ClientWithPlaceholderPOC

open FSharp.Quotations

open WebSharper
open WebSharper.UI
open WebSharper.Web

type InlineControlWithPlaceHolder(docExpr: Expr<Doc>, doc: Doc) =
    inherit InlineControl<Doc>(docExpr)

    [<System.NonSerialized>]
    let doc = doc

    // this is needed because WebSharper.Web.Control.GetBodyNode looks at Body property on current type
    [<JavaScript>]
    override this.Body = base.Body

    interface INode with
        member this.Write (ctx, w) =
            w.Write("""<div id="{0}">""", this.ID)
            doc.Write(ctx, w, None)
            w.Write("</div>")

type ClientDoc =
    static member withPlaceholder ([<ReflectedDefinition(true); JavaScript>] expr: Expr<Doc>) =
        match expr with
        | Patterns.WithValue(doc, _, docExpr) ->
            ConcreteDoc(INodeDoc (new InlineControlWithPlaceHolder (Expr.Cast<Doc> docExpr, doc :?> Doc))) :> Doc
        | _ ->
            // value missing, nothing to render on server
            Html.client expr

