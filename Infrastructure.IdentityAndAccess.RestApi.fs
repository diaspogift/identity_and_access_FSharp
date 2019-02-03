namespace IdentityAndAcccess.ReastApi


open System
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave.Successful
open Suave
open Suave.Operators
open Suave.Filters

open IdentityAndAcccess.Commamds.Handlers

open IdentityAndAcccess.Workflow.ProvisionTenantApiTypes
open IdentityAndAcccess.Workflow.AssignGroupToRoleApiTypes
open IdentityAndAcccess.Workflow.DeactivateTenantActivationStatusApiTypes
open IdentityAndAcccess.Workflow.OffertRegistrationInvitationApiTypes
open IdentityAndAcccess.Workflow.ProvisionGroupApiTypes
open IdentityAndAcccess.Workflow.ProvisionRoleApiTypes
open IdentityAndAcccess.Workflow.ReactivateTenantActivationStatusApiTypes
open IdentityAndAcccess.Workflow.WithdrawRegistrationInvitationApiTypes
open IdentityAndAcccess.Commamds.Handlers
open RabbitMQ.Client.Impl

open IdentityAndAcccess.Workflow.ProvisionRoleApiTypes
open IdentityAndAcccess.Workflow.AssignGroupToRoleApiTypes

open IdentityAndAcccess.Workflow.AddUserToGroupApiTypes
open IdentityAndAcccess.Workflow.AddUserToGroupApiTypes.AddUserToGroupWorfklowImplementation


open IdentityAndAcccess.Workflow.RegisterUserApiTypes
open IdentityAndAcccess.Workflow.RegisterUserApiTypes.RegisterUserToTenancyWorfklowImplementation



open IdentityAndAcccess.Workflow.AssignUserToRoleApiTypes
open IdentityAndAcccess.Workflow.AssignUserToRoleApiTypes.AssignUserToRoleWorfklowImplementation
open IdentityAndAcccess.Commamds.Handlers.Command
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
        let settings = JsonSerializerSettings()
        settings.ContractResolver <- CamelCasePropertyNamesContractResolver()
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

            path "/register-users" 
                >=> request (getResourceFromReq<UnalidatedUserTenancy> 
                >> Command.RegisterUser.toCommand 
                >> Command.RegisterUser.handle
                >> workflowResultToHttpReponse
                >> toJsonWebPart)

            path "/activate-tenant" 
                >=> request (getResourceFromReq<UnvalidatedTenantActivationStatusData> 
                >> Command.ReactivateTenant.toCommand 
                >> Command.ReactivateTenant.handle
                >> workflowResultToHttpReponse
                >> toJsonWebPart)

            path "/deactivate-tenant" 
                >=> request (getResourceFromReq<UnvalidatedTenantDeactivationStatus> 
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


            path "/assign-user-to-role" 
                >=> request (getResourceFromReq<UnvalidatedRoleAndUserId> 
                >> Command.AssignUserToRole.toCommand
                >> Command.AssignUserToRole.handle
                >> workflowResultToHttpReponse
                >> toJsonWebPart)

            path "/assign-group-to-role" 
                >=> request (getResourceFromReq<UnvalidatedRoleAndGroupIds> 
                >> Command.AssignGroupToRole.toCommand
                >> Command.AssignGroupToRole.handle
                >> workflowResultToHttpReponse
                >> toJsonWebPart)

            path "/add-user-to-group" 
                >=> request (getResourceFromReq<UnvalidatedGroupAndUserId> 
                >> Command.AddUserToGroup.toCommand
                >> Command.AddUserToGroup.handle
                >> workflowResultToHttpReponse
                >> toJsonWebPart)            
                   
            path "/assign-group-to-role" 
                >=> request (getResourceFromReq<UnvalidatedRoleAndGroupIds> 
                >> Command.AssignGroupToRole.toCommand
                >> Command.AssignGroupToRole.handle
                >> workflowResultToHttpReponse
                >> toJsonWebPart)             
                   ]
                ]