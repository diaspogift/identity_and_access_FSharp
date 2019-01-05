﻿// Learn more about F# at http://fsharp.org




open Suave.Web
open Suave.Successful
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAccess.DatabaseTypes.Functions
open MongoDB.Bson
open IdentityAndAcccess.DomainServices
open IdentityAndAccess.DatabaseTypes.Functions
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAccess.DatabaseTypes.Functions
open IdentityAndAccess.DatabaseTypes.Functions
open IdentityAndAccess.DatabaseTypes
open IdentityAndAccess.DatabaseTypes.Functions
open IdentityAndAccess.DatabaseTypes.Functions
open IdentityAndAcccess.DomainTypes.Functions
open System
open IdentityAndAcccess.DomainServices.Group
open IdentityAndAccess.DatabaseTypes.Functions



let DomainServices:GroupMemberService = {TimeServiceWasCalled = DateTime.Now; CallerCredentials = (CallerCredential "Felicien")}


(*
let rs = result {


                let! user = User.create "1" "1" "F" "M" "L" "email@gmail.com" "address" "669262656" "669272757" "username" "my_current_password"
                printfn "User password before changed = %A" user.Password

                
                let! newPassword = Password.create "password" "my_new_password"  
                let! currentPassword = Password.create "current password" "my_current_password"  

                let! userWithChangedPassword = User.changePassWord user currentPassword newPassword

                return userWithChangedPassword
}

match rs with 
| Ok success -> printfn "New Password Here = %A" success.Password
| Error error -> printfn "Error = %A" error






printfn "----------------------------------------------" 
printfn "----------------------------------------------" 
printfn "----------------------------------------------" 










let rsChangeContactInformation = result {


                let! user = User.create "1" "1" "F" "M" "L" "email@gmail.com" "address" "669262656" "669272757" "username" "my_current_password"
                printfn "User contact info before changed = %A" user.Person.Contact


                let! newEmail = EmailAddress.create "email" "changed_email@gmail.com"
                let! newPostalAddreess = PostalAddress.create "address" "changed_address"
                let! newPrimaryTel = Telephone.create "primaryTel" "699676756"
                let! newSecondaryTel = Telephone.create "secondaryTel" "699676759"
                
                let newContactInformation = {
                        Email = newEmail
                        Address = newPostalAddreess
                        PrimaryTel = newPrimaryTel
                        SecondaryTel = newSecondaryTel 
                       
                        }  

                let userWithChangedContactInformation = User.changePersonalContactInformation user newContactInformation

                return userWithChangedContactInformation
}


match rsChangeContactInformation with 
| Ok success -> printfn "New contact info Here = %A" success.Person.Contact
| Error error -> printfn "Error = %A" error












printfn "NEWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW" 
printfn "NEWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW" 
printfn "NEWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW" 
*)


(*let rs1 = result {


                let! user1 = User.create "507f1f77bcf86cd799439014" "40" "Precilia" "N/A" "FOTIO MELING" "meling.hess@gmail.com" "Denver, Bonamoussadi Duoala" "669262656" "669272757" "meling" "my_current_password"
                let! user2 = User.create "507f1f77bcf86cd799439015" "40" "Birrito" "N/A" "FOTIO HESS" "burrito.hess@gmail.com" "Denver, Bonamoussadi Duoala" "669262656" "669272757" "burito" "my_current_password"

                let saveOneUserDependantFunction = UserDb.saveOneUser
                let loadUserByDependantFunction = UserDb.loadOneUserById
                let userDtoToSave1 = DbHelpers.fromUserDomainToDto user1
                let userDtoToSave2 = DbHelpers.fromUserDomainToDto user2

                saveOneUserDependantFunction userDtoToSave1
                saveOneUserDependantFunction userDtoToSave2


                let id = new BsonObjectId(new ObjectId((UserId.value user1.UserId)))

                printfn "JUST BEFORE THE CALL"

                let savedUserFromDb = loadUserByDependantFunction id

                return! savedUserFromDb |> DbHelpers.fromDbDtoToUser
}

match rs1 with 
| Ok aUser -> printfn "User that was saved is here = %A" aUser
| Error error -> printfn "Error = %A" error

*)




(*let rsCreateUserSaveAndTryToReloadItFormDb = result {


                let! user1 = User.create "507f1f77bcf86cd799439014" "400" "Precilia new" "N/A new" "FOTIO MELING new" "meling_new.hess@gmail.com" "Denver new, Bonamoussadi Duoala" "669262659" "669272759" "new_meling" "new_my_current_password"

                let userDtoToSave1 = DbHelpers.fromUserDomainToDto user1
                let updateUserDependency = UserDb.updateOneUser
                let loadUserByDependantFunction = UserDb.loadOneUserById


                printfn "JUST BEFORE THE CALL 1111111111111111111"

                updateUserDependency userDtoToSave1

                let updatedUserFromDb = loadUserByDependantFunction userDtoToSave1.UserId

                return! updatedUserFromDb |> DbHelpers.fromDbDtoToUser
}

match rsCreateUserSaveAndTryToReloadItFormDb with 
| Ok aUser -> printfn "User that was update username is here = %A" aUser.Username
| Error error -> printfn "Error = %A" error*)



(*let rsCreateTenantSaveAndTryToReloadItFormDb = result {


                let! tenant = Tenant.create "507f1f77bcf86cd799439010" "Mobile Biller new" "Money Tranfer Solutions new"
                
                
                let tenantToSave  = DbHelpers.fromTenantDomainToDto tenant
                let saveTenantDependencyFunction = TenantDb.saveOneTenant
                let updateTenantDependencyFunction = TenantDb.updateOneTenant
                let loadTenantByIdDependancyFunction = TenantDb.loadOneTenantById


                //printfn "JUST BEFORE THE CALL 22222222222222222222222222"

                //saveTenantDependencyFunction tenantToSave

                //printfn "JUST AFTER THE CALL 22222222222222222222222222"


                let toBisonId (aTenantId : TenantId) = 

                        new BsonObjectId(new ObjectId((TenantId.value aTenantId))) 
                        

                updateTenantDependencyFunction tenantToSave


                printfn "JUST AFTER THE CALL 22222222222222222222222222 THE LOADED TENANT FROM DB : %A" tenantToSave



                return tenantToSave 


}

match rsCreateTenantSaveAndTryToReloadItFormDb with 
| Ok aTenant
        -> printfn "The tenant that was create is here: Name is  = %A" aTenant.Name
           printfn "The full tenant = %A" aTenant.Name
| Error error 
        -> printfn "Error = %A" error*)


(*let rsChangeTenantActivationStatus = result {

                let toBisonId (aTenantId : TenantId) = 
                        new BsonObjectId(new ObjectId((TenantId.value aTenantId))) 

                let! tenant = Tenant.create "507f1f77bcf86cd799439010" "Mobile Biller new" "Money Tranfer Solutions new"
                
                
                //let tenantToactevateDto  = TenantDb.loadOneTenantById (tenant.TenantId |> toBisonId)

                //let! tenantToactevate = DbHelpers.fromDbDtoToTenant tenantToactevateDto

                //printfn "------------ HERE tenantToActive that must be deactivated: == %A ------------  " tenantToactevate

                //let! activatedTenant = Services.activateTenant tenantToactevate

                //printfn "------------ HERE activatedTenant that has been activated: == %A ------------ " activatedTenant

                
                //let saveTenantDependencyFunction = TenantDb.saveOneTenant
                let updateTenantDependencyFunction = TenantDb.updateOneTenant
                //let loadTenantByIdDependancyFunction = TenantDb.loadOneTenantById


                //printfn "JUST BEFORE THE CALL 22222222222222222222222222"

                //saveTenantDependencyFunction tenantToSave

                //printfn "JUST AFTER THE CALL 22222222222222222222222222"


                let toBisonId (aTenantId : TenantId) = 

                        new BsonObjectId(new ObjectId((TenantId.value aTenantId))) 
                

                //let tenantDto = DbHelpers.fromTenantDomainToDto deactivatedTenant

                printfn "----------------------------"
                //printfn "Converted TENANT TO TENANTDTO : %A" tenantDto
                printfn "----------------------------"


                //updateTenantDependencyFunction (activatedTenant |> DbHelpers.fromTenantDomainToDto)


                //printfn "JUST AFTER THE CALL 22222222222222222222222222 THE LOADED TENANT FROM DB : %A" deactivatedTenant
                let! registrationDescription = RegistrationInvitationDescription.create "registration description" "To register"
                let! ten = Tenant.offerRegistrationInvitation tenant registrationDescription

                updateTenantDependencyFunction (ten |> DbHelpers.fromTenantDomainToDto)

                return ten 


}

match rsChangeTenantActivationStatus with 
| Ok aTenant
        -> printfn "The tenant that offered a registration invitation  = %A" aTenant.Name
           printfn "The full tenant = %A" aTenant
| Error error 
        -> printfn "Error = %A" error*)


let rsCreateGroupSaveAndTryToReloadItFormDb = result {


                let! groupToAdd1 = Group.create "507f1f77bcf86cd799439051" "507f1f77bcf86cd799439010" "Distributeur" "Distributeur" []
                let! groupToAdd2 = Group.create "24e7538d90ad4bd7a448d152" "507f1f77bcf86cd799439010" "Registarts" "Registarts" []
                let! groupToAdd3 = Group.create "24e7538d90ad4bd7a448d153" "507f1f77bcf86cd799439010" "Cleaners" "Cleaners" []
                let! groupToAdd4 = Group.create "24e7538d90ad4bd7a448d154" "507f1f77bcf86cd799439010" "Formateurs" "Formateurs" []
                let! groupToAdd5 = Group.create "24e7538d90ad4bd7a448d155" "507f1f77bcf86cd799439010" "Urgenciers" "Urgenciers" []
                
                let groupToSave1 = DbHelpers.fromGroupDomainToDto groupToAdd1
                let groupToSave2 = DbHelpers.fromGroupDomainToDto groupToAdd2
                let groupToSave3 = DbHelpers.fromGroupDomainToDto groupToAdd3
                let groupToSave4 = DbHelpers.fromGroupDomainToDto groupToAdd4
                let groupToSave5 = DbHelpers.fromGroupDomainToDto groupToAdd5
              

                let saveGroupDependencyFunction = GroupDb.saveOneGroup
                let updateGroupDependencyFunction = GroupDb.updateOneGroup
                let loadGroupByIdDependancyFunction = GroupDb.loadOneGroupById

                //saveGroupDependencyFunction groupToSave1
                //saveGroupDependencyFunction groupToSave2
                //saveGroupDependencyFunction groupToSave3
                //saveGroupDependencyFunction groupToSave4
                //saveGroupDependencyFunction groupToSave5

                let id = (new BsonObjectId(new ObjectId(GroupId.value groupToAdd1.GroupId)))
                let groupToAddToDto = loadGroupByIdDependancyFunction id
                let! groupToAddTo = groupToAddToDto |> DbHelpers.fromDbDtoToGroup 



                printfn "GROUP LOADED = %A" groupToAddToDto
                printfn "GROUP TO SAVE = %A" groupToAddTo

           
                let! groupWithMember = Group.addGroupToGroup groupToAddTo groupToAdd5 DomainServices.IsMemberGroupMember

                printfn "HERE THE TIME THE isMemberGroupMember was called at : %A " DomainServices.TimeServiceWasCalled
                

                let groupToSave  = DbHelpers.fromGroupDomainToDto groupWithMember
                updateGroupDependencyFunction groupToSave

                

                return groupToAdd1 


}

match rsCreateGroupSaveAndTryToReloadItFormDb with 
| Ok aGroup
        -> printfn "The adapted result is here  = %A" aGroup
| Error error 
        -> printfn "Error = %A" error























(*let rsChangeTenantActivationStatus = result {

                let toBisonId (aTenantId : TenantId) = 
                        new BsonObjectId(new ObjectId((TenantId.value aTenantId))) 

                let! tenant = Tenant.create "507f1f77bcf86cd799439010" "Mobile Biller new" "Money Tranfer Solutions new"
                
                
               
                let saveGroupDependencyFunction = GroupDb.saveOneGroup
              

                let toBisonId (aTenantId : TenantId) = 

                        new BsonObjectId(new ObjectId((TenantId.value aTenantId))) 
                

                //let tenantDto = DbHelpers.fromTenantDomainToDto deactivatedTenant

                printfn "----------------------------"
                //printfn "Converted TENANT TO TENANTDTO : %A" tenantDto
                printfn "----------------------------"


                //updateTenantDependencyFunction (activatedTenant |> DbHelpers.fromTenantDomainToDto)


                //printfn "JUST AFTER THE CALL 22222222222222222222222222 THE LOADED TENANT FROM DB : %A" deactivatedTenant
                let! groupName = GroupName.create "group name" "Promoteurs"
                let! groupDescription = GroupDescription.create "group description" "Promoteurs de nos produits"

                let! group = Tenant.provisionGroup tenant groupName groupDescription


                printfn "HERE THE PROVISIONED GROUP : %A" group


                saveGroupDependencyFunction (group |> DbHelpers.fromGroupDomainToDto)

                return group 


}

match rsChangeTenantActivationStatus with 
| Ok group
        -> printfn "The group that was provisioned  = %A" group.Name
           printfn "The full group = %A" group
| Error error 
        -> printfn "Error = %A" error*)














(*let rsChangeTenantActivationStatus = result {

            
                let! tenant = Tenant.create "b6a0d78a41ef4d8ea1c51772" "Mobile Biller new" "Money Tranfer Solutions new"
                
                
               
                let saveRoleDependencyFunction = RoleDb.saveOneRole
                let updateRoleDependencyFunction = RoleDb.updateOneRole
                let loadRoleByIdDependencyFunction = RoleDb.loadOneRoleById
              

                let toBisonId (aTenantId : TenantId) = 
                        new BsonObjectId(new ObjectId((TenantId.value aTenantId))) 
        
                let! roleName = RoleName.create  "role name" "UpdatedAdministrateurPrincipaux"
                let! roleDescription = RoleDescription.create "role description" "Updated Administrateur principaux de diaspogift"


                let roleDto : RoleDto = loadRoleByIdDependencyFunction ( tenant.TenantId |> toBisonId)


                printfn "HERE THE LOADED ROLE : %A" roleDto

                let! role = roleDto |> DbHelpers.fromDtoToRoleDomain

                let updatedRole = { role with Name = roleName; Description = roleDescription}

                printfn "HERE THE LOADED ROLE UPDATED : %A" updatedRole


                updatedRole
                |> DbHelpers.fromRoleDomainToDto
                |> updateRoleDependencyFunction  



                //saveRoleDependencyFunction (role |> DbHelpers.fromDtoToRoleDomain)

                return role 


}

match rsChangeTenantActivationStatus with 
| Ok role
        -> printfn "The role that was loaded  = %A" role.Name
           printfn "The full role = %A" role
| Error error 
        -> printfn "Error = %A" error*)



[<EntryPoint>]
let main argv =

   startWebServer defaultConfig (OK "hello")
   0