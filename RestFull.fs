namespace SuaveRestApi.Rest

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave.Successful
open Suave
open Suave.Operators
open Suave.Filters




[<AutoOpen>]
module Restful =

    ///Transform an type 'a into a webpart
    let JSON v = 
        let settings = new JsonSerializerSettings()
        settings.ContractResolver <- new CamelCasePropertyNamesContractResolver()

        JsonConvert.SerializeObject(v, settings)
        |> OK >=> Writers.setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json = 
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a
    

    let getResourceFromReq<'a> (req : HttpRequest) = 
        req.rawForm 
        |> System.Text.Encoding.UTF8.GetString 
        |> fromJson<'a>



    type RestResource<'a> = {

        GetAll : unit -> 'a seq
        Create : 'a -> unit
        Update : 'a -> unit
        Delete : int -> unit
        GetById : int -> 'a option
    }

    
    let rest resourceName resource = 
        let resourcePath = "/" + resourceName


        let resourceIdPath = 
            let path = resourcePath + "/%d"
            new PrintfFormat<(int -> string), unit, string, string, int>(path)


        //printfn "Here is the path ????????????????? == ????????????????? %A" resourceIdPath


        let deleteResourceById id =
            resource.Delete id
            NO_CONTENT
        let getResourceById id =
            resource.GetById id |>  JSON
            
        
        
        let getAll = warbler( fun _ -> resource.GetAll() |> JSON )

        choose [
                path resourcePath >=> choose [
                                        GET >=> getAll
                                        POST >=> request (getResourceFromReq >> resource.Create >> JSON) >=> NO_CONTENT
                                        PUT >=> request (getResourceFromReq >> resource.Update >> JSON) >=> NO_CONTENT
                                      ]
                DELETE >=> (pathScan resourceIdPath deleteResourceById )
                GET >=> (pathScan resourceIdPath getResourceById )
        ]