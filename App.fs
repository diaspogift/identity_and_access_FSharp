﻿// Learn more about F# at http://fsharp.org




open Suave.Web
open Suave.Successful
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAccess.DatabaseTypes.Functions
open MongoDB.Bson
open IdentityAndAccess.DatabaseTypes.Functions
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAccess.DatabaseTypes.Functions
open IdentityAndAccess.DatabaseTypes.Functions



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


(*let rsCreateGroupSaveAndTryToReloadItFormDb = result {


                let! group = Group.create "507f1f77bcf86cd799439051" "507f1f77bcf86cd799439010" "EmployesStageres" "A group containing interns employees like the ones in Internship programs ...etc ..." []
                
                
                let groupToSave  = DbHelpers.fromGroupDomainToDto group
                let saveGroupDependencyFunction = GroupDb.saveOneGroup
                let updateGroupDependencyFunction = GroupDb.updateOneGroup
                let loadGroupByIdDependancyFunction = GroupDb.loadOneGroupById


               // printfn "JUST BEFORE THE CALL 22222222222222222222222222"

                //saveGroupDependencyFunction groupToSave

               // printfn "JUST AFTER THE CALL 22222222222222222222222222 THE PARAM TO SAVEONEGOUP INTO GROUP DB : %A" groupToSave

                //printfn "JUST AFTER THE CALL 22222222222222222222222222"


                let groupToUpdate = groupToSave

                let toBisonId (aGroupId : GroupId) = 
                        new BsonObjectId(new ObjectId((GroupId.value aGroupId))) 
                        

                updateGroupDependencyFunction groupToUpdate


                //printfn "JUST AFTER THE CALL 33333333333333333333333333 THE UPDATED UPDATEGROUP INTO GROUP DB : %A" groupToUpdate

                //let loadedGroupFromDb = loadGroupByIdDependancyFunction (toBisonId group.GroupId)


                return groupToUpdate 


}

match rsCreateGroupSaveAndTryToReloadItFormDb with 
| Ok aGroup
        -> printfn "The result is here  = %A" aGroup
| Error error 
        -> printfn "Error = %A" error*)























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














let rsChangeTenantActivationStatus = result {

                let toBisonId (aTenantId : TenantId) = 
                        new BsonObjectId(new ObjectId((TenantId.value aTenantId))) 

                let! tenant = Tenant.create "507f1f77bcf86cd799439010" "Mobile Biller new" "Money Tranfer Solutions new"
                
                
               
                let saveRoleDependencyFunction = RoleDb.saveOneRole
              

                let toBisonId (aTenantId : TenantId) = 

                        new BsonObjectId(new ObjectId((TenantId.value aTenantId))) 
                

                //let tenantDto = DbHelpers.fromTenantDomainToDto deactivatedTenant

                printfn "----------------------------"
                //printfn "Converted TENANT TO TENANTDTO : %A" tenantDto
                printfn "----------------------------"


                //updateTenantDependencyFunction (activatedTenant |> DbHelpers.fromTenantDomainToDto)


                //printfn "JUST AFTER THE CALL 22222222222222222222222222 THE LOADED TENANT FROM DB : %A" deactivatedTenant
                let! roleName = RoleName.create "role name" "AdministrateurPrincipaux"
                let! roleDescription = RoleDescription.create "role description" "Administrateur principaux de diaspogift"

                //let! group = Tenant.provisionGroup tenant groupName groupDescription


                //printfn "HERE THE PROVISIONED GROUP : %A" group


                //saveGroupDependencyFunction (group |> DbHelpers.fromGroupDomainToDto)

                let! role = Tenant.provisionRole tenant roleName roleDescription

                

                saveRoleDependencyFunction (role |> DbHelpers.fromDtoToRoleDomain)

                return role 


}

match rsChangeTenantActivationStatus with 
| Ok role
        -> printfn "The role that was provisioned  = %A" role.Name
           printfn "The full role = %A" role
| Error error 
        -> printfn "Error = %A" error



[<EntryPoint>]
let main argv =

   startWebServer defaultConfig (OK "hello")
   0