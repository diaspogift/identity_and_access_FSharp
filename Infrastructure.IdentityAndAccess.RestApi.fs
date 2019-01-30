namespace IdentityAndAcccess.ReastApi


open System
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave.Successful
open Suave
open Suave.Operators
open Suave.Filters


open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Commamds.Handlers
open RabbitMQ.Client.Impl

open IdentityAndAcccess.Workflow.ProvisionRoleApiTypes
open IdentityAndAcccess.Workflow.AddGroupToGroupApiTypes





[<AutoOpen>]
module Rest =



    type JsonString = string

    /// Very simplified version!
    type HttpResponse = {
        HttpStatusCode : int
        Body : JsonString 
        }


/// This function converts the workflow output into a HttpResponse
    let workflowResultToHttpReponse result = 
        match result with
        | Ok events ->
            // and serialize to JSON
            let json = JsonConvert.SerializeObject(events)
            let response = 
                {
                HttpStatusCode = 200
                Body = json
                }
            response
        | Error err -> 

            // turn domain errors into a dto
            //let dto = err |> PlaceOrderErrorDto.fromDomain
            // and serialize to JSON
            let json = JsonConvert.SerializeObject(err)
            let response = 
                {
                HttpStatusCode = 401
                Body = json
                }
            response




            
    ///Transform an type 'a into a webpart
    let toJsonWebPart v = 
        let settings = new JsonSerializerSettings()
        settings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
        JsonConvert.SerializeObject(v, settings)
        |> OK >=> Writers.setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json = JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a
    

    let getResourceFromReq<'a> (req : HttpRequest) = 
        req.rawForm 
        |> System.Text.Encoding.UTF8.GetString 
        |> fromJson<'a>


    let app = choose [
        POST >=> choose [

            path "/tenant-provisions" 
                >=> request (getResourceFromReq<UnvalidatedTenantProvision> 
                >> Command.ProvisionTenant.toCommand 
                >> Command.ProvisionTenant.handle 
                >> workflowResultToHttpReponse
                >> toJsonWebPart)

            path "/offert-invitations" 
                >=> request (getResourceFromReq<UnvalidatedRegistrationInvitationDescription> 
                >> Command.OfferInvitation.toCommand 
                >> Command.OfferInvitation.handle
                >> workflowResultToHttpReponse
                >> toJsonWebPart)

            path "/withdraw-invitations" 
                >=> request (getResourceFromReq<UnvalidatedRegistrationInvitationIdentifier> 
                >> Command.WithdrawInvitation.toCommand 
                >> Command.WithdrawInvitation.handle
                >> workflowResultToHttpReponse
                >> toJsonWebPart)

            path "/activate-tenant" 
                >=> request (getResourceFromReq<UnvalidatedTenantActivationStatusData> 
                >> Command.ReactivateTenant.toCommand 
                >> Command.ReactivateTenant.handle
                >> workflowResultToHttpReponse
                >> toJsonWebPart)

            path "/deactivate-tenant" 
                >=> request (getResourceFromReq<UnvalidatedTenantActivationStatus> 
                >> Command.DeactivateTenant.toCommand
                >> Command.DeactivateTenant.handle
                >> workflowResultToHttpReponse
                >> toJsonWebPart) 

            path "/provision-group" 
                >=> request (getResourceFromReq<UnvalidatedGroup> 
                >> Command.ProvisionGroup.toCommand
                >> Command.ProvisionGroup.handle
                >> workflowResultToHttpReponse
                >> toJsonWebPart)   

            path "/provision-role" 
                >=> request (getResourceFromReq<UnvalidatedRole> 
                >> Command.ProvisionRole.toCommand
                >> Command.ProvisionRole.handle
                >> workflowResultToHttpReponse
                >> toJsonWebPart)

            (* path "/add-user-to-group" 
                >=> request (getResourceFromReq<UnvalidatedRole> 
                >> Command.ProvisionRole.toCommand
                >> Command.ProvisionRole.handle
                >> workflowResultToHttpReponse
                >> toJsonWebPart)  *)            
                   
            path "/add-group-to-group" 
                >=> request (getResourceFromReq<UnvalidatedGroupIds> 
                >> Command.AddGroupToGroup.toCommand
                >> Command.AddGroupToGroup.handle
                >> workflowResultToHttpReponse
                >> toJsonWebPart)             
                   ]
                ]