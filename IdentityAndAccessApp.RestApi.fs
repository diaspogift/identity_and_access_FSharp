namespace IdentityAndAcccess.ReastApi


open System
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave.Successful
open Suave
open Suave.Operators
open Suave.Filters


open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.DomainApiTypes.Handlers
open RabbitMQ.Client.Impl




[<AutoOpen>]
module Rest =

    ///Transform an type 'a into a webpart
    let JSON v = 
        let settings = new JsonSerializerSettings()
        settings.ContractResolver <- new CamelCasePropertyNamesContractResolver()

        JsonConvert.SerializeObject(v, settings)
        |> OK >=> Writers.setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json = 
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a
    

    let getResourceFromReq<'a> (req : HttpRequest) = 

        let data = req.rawForm 
                   |> System.Text.Encoding.UTF8.GetString 
                   |> fromJson<'a>
        data





    let toProvisionTenantCommand (up:UnvalidatedTenantProvision) : ProvisionTenantCommand =

        printfn "ppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppp"
        printfn "ppppppppppppppppppppp %A pppppppppppppppppppppppppppppppppppppppppppppppppppppppppp" up
        printfn "ppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppp"
        
        let provisionTenantCommand : ProvisionTenantCommand = {
            Data = up
            TimeStamp = DateTime.Now
            UserId = "Felicien"
            }
        provisionTenantCommand
 

    let app = choose [
                POST >=> choose [
                            path "/tenant-provisions" 
                                >=> request (getResourceFromReq<UnvalidatedTenantProvision> 
                                >> toProvisionTenantCommand >> ProvisionTenant.handleProvisionTenant 
                                >> JSON)
                            // path "activate-tenant" 
                            //     >=> request (getResourceFromReq >> resource.Create >> JSON)
                            //     >=> request (getResourceFromReq >> resource.Update >> JSON)
                            // path "deactivate-tenant" 
                            //     >=> request (getResourceFromReq >> resource.Create >> JSON)
                            //     >=> request (getResourceFromReq >> resource.Update >> JSON)
                            // path "offert-invitations" 
                            //     >=> request (getResourceFromReq >> resource.Create >> JSON)
                            //     >=> request (getResourceFromReq >> resource.Update >> JSON)
                            // path "withdraw-invitations" 
                            //     >=> request (getResourceFromReq >> resource.Create >> JSON)
                            //     >=> request (getResourceFromReq >> resource.Update >> JSON)
                           ]
                ]