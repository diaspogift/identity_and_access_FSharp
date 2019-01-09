module IdentityAndAcccess.DomainServices

open System
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation
open IdentityAndAcccess.DomainTypes.Functions

open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.DomainTypes.Role








let unwrapToStandardGroup aGroupToUnwrapp = 
        match aGroupToUnwrapp with 
            | Standard aStandardGroup -> aStandardGroup
            | Internal anInternalGroup -> anInternalGroup




///User type related services 
/// 
///  
module Tenant =


    ///Database dependecies 
    
    let saveOneTenantDbDependencyFunction = TenantDb.saveOneTenant
    let saveOneUserDbDependencyFunction = TenantDb.saveOneTenant
    let saveOneRoleDbDependencyFunction = TenantDb.saveOneTenant

    ///Other domain service dependencies 
    
    let strongPasswordServiceImpl:StrongPasswordGeneratorService = 
        fun aPassword ->
            let unwrappedPassword = Password.value aPassword
            //some logic go he to generate a strong password
            StrongPassword.create' unwrappedPassword




    let passwordEncryptionServiceImpl:PasswordEncryptionService = 
        fun aStrongPassword ->
            let unwrappedPassword = StrongPassword.value aStrongPassword
            //some logic go he to generate a strong password
            EncrytedPassword.create' unwrappedPassword






    let provisionTenantServiceImpl : ProvisionTenantService =
                                     
        fun strongPasswordService passwordEncryptionService aTenantName aTenantDescription anAdministorFirstName 
            anAdministorMiddleName anAdministorLastName anEmailAddress aPostalAddress aPrimaryTelephone aSecondaryTelephone ->



          result {

            let enablementSartDate = DateTime.Now
            let enablementEndDate = enablementSartDate.AddDays(365.0)
            let!  invitationDescription = "Invitation for Tenant ..."  |> RegistrationInvitationDescription.create' 

            let! tenantToProvision = Tenant.createFullActivatedTenant aTenantName aTenantDescription

            printfn "------------------------------------------------------------------------------" 

            //printfn "tenantToProvision =                %A" tenantToProvision
            printfn "------------------------------------------------------------------------------" 


            let! tenantWithRegistrationInvitation, registrationInvitation =  Tenant.offerRegistrationInvitation  tenantToProvision  invitationDescription

            printfn "------------------------------------------------------------------------------" 

            printfn "tenantWithRegistrationInvitation =                 %A" tenantWithRegistrationInvitation

            printfn "------------------------------------------------------------------------------" 

            let! password = Password.create' "123456"

            printfn "password =                 %A" password

            printfn "------------------------------------------------------------------------------" 


            let!  strongPassword = password |> strongPasswordService


            printfn "strongPassword =                 %A" strongPassword


            printfn "------------------------------------------------------------------------------" 


            let! encryptedPassword = strongPassword |> passwordEncryptionService

            printfn "encryptedPassword =                 %A" encryptedPassword

            printfn "------------------------------------------------------------------------------" 


            let! unwrappedEncryptedPassword = encryptedPassword 
                                           |> EncrytedPassword.value 
                                           |> Password.create'


            printfn "unwrappedEncryptedPassword =                 %A" unwrappedEncryptedPassword


            printfn "------------------------------------------------------------------------------" 


            let! adminUsername = Username.create' "Default Aministrator"

            printfn "------------------------------------------------------------------------------" 


            printfn "adminUsername =                %A" adminUsername

            printfn "------------------------------------------------------------------------------" 

            let! adminUserEnablement = Enablement.fullCreate enablementSartDate enablementEndDate EnablementStatus.Enabled

            printfn "------------------------------------------------------------------------------" 

            printfn "adminUserEnablement =                %A" adminUserEnablement

            printfn "------------------------------------------------------------------------------" 


            let! adminUser = Tenant.registerUserForTenant tenantWithRegistrationInvitation registrationInvitation.RegistrationInvitationId adminUsername  
                                                          unwrappedEncryptedPassword adminUserEnablement anEmailAddress aPostalAddress aPrimaryTelephone 
                                                          aSecondaryTelephone anAdministorFirstName anAdministorMiddleName anAdministorLastName 

            printfn "adminUser =           %A" adminUser


                                        
            let! rsWithdrawInvitation = Tenant.withdrawInvitation tenantWithRegistrationInvitation registrationInvitation.RegistrationInvitationId


            let! adminRoleName = "SUPER_ADMINISTRATOR" |> RoleName.create'
            let! adminRoleDescription = "SUPER_ADMINISTRATOR is a role that have access to all tenant'rsources" |> RoleDescription.create'

            let! adminRole = Tenant.provisionRole rsWithdrawInvitation adminRoleName adminRoleDescription

            let resultAssignUserToRole = Role.assignUser adminRole adminUser

            //IO operation kept and the end ????


            return (tenantToProvision, adminUser, adminRole)
          }


    let provisionTenantServiceImpl' = provisionTenantServiceImpl strongPasswordServiceImpl passwordEncryptionServiceImpl



///Group type related services 
/// 
///  
module Group =





    ///Database dependecies 
     
    
    let loadGroupByIdDbDependencyFunction = GroupDb.loadOneGroupById


    let loadGroupByGroupMemberIdDbDependencyFunction = GroupDb.loadOneGroupMemberById
     

     
                



    ///services
    
    
    let isGroupMemberIsInGroupServiceImplLocal  =

        fun loadGroupMemberById  // Database dependency function
            aGroup 
            aMember -> 
                
            let aStandardGroup = aGroup 
                                 |> DomainHelpers.unwrapToStandardGroup


                    
            let rec InternalRecursiveIsGroupMemberIsInGroupService  
                                                                    (aMember : GroupMember)  
                                                                    (loadGroupMemberById : LoadGroupMemberById) 
                                                                    (aGroupMemberList : GroupMember list) =

                    match aGroupMemberList with
                    | [] -> 
                        false
                    | head::tail ->

                        if (head.Type = GroupGroupMember) && (head = aMember) then
                            true
                        else 
                        
                        ///IO operation here. looking for a group member by its group member identifier - START
                            let additionalGroupToSearch = loadGroupMemberById head.MemberId
                        ///IO operation here. - END
                       
                            match additionalGroupToSearch with
                            | Ok anAdditionalGroupToSearch ->

                                let aStandardAdditionalGroupToSearch = anAdditionalGroupToSearch 
                                                                       |> DomainHelpers.unwrapToStandardGroup
                                                                       
                                let newMembersToAppend =  aStandardAdditionalGroupToSearch.Members
                                let allMembers = tail @ newMembersToAppend
                                let result = InternalRecursiveIsGroupMemberIsInGroupService aMember loadGroupMemberById allMembers

                                result

                            | Error _ -> false


            InternalRecursiveIsGroupMemberIsInGroupService  aMember  loadGroupMemberById   aStandardGroup.Members







    let isUserInNestedGroupServiceImpl : IsUserInNestedGroupService = 

        fun  
            loadGroupMemberById  //Database dependency
            aGroup 
            aUser ->




            let rec InternalRecursiveIsUserInNestedGroupService  
                                                                (aMember : GroupMember)  
                                                                (loadGroupMemberById : LoadGroupMemberById) 
                                                                (aGroupMemberList : GroupMember list) =

                    match aGroupMemberList with
                    | [] -> 
                        false
                    | head::tail ->

                        if (head.Type = GroupGroupMember) && (head = aMember) then
                            true
                        else 
                        
                        
                        ///IO operation here. looking for a group member by its group member identifier - START
                            let additionalGroupToSearch = loadGroupMemberById head.MemberId
                        ///IO operation here. - END
                       
                            match additionalGroupToSearch with
                            | Ok anAdditionalGroupToSearch ->

                                let aStandardAdditionalGroupToSearch = anAdditionalGroupToSearch 
                                                                       |> DomainHelpers.unwrapToStandardGroup
                                                                       
                                let newMembersToAppend =  aStandardAdditionalGroupToSearch.Members
                                let allMembers = tail @ newMembersToAppend
                                let result = InternalRecursiveIsUserInNestedGroupService aMember loadGroupMemberById allMembers

                                result

                            | Error _ -> false


            

            


            let aStandardGroup = aGroup 
                                 |> DomainHelpers.unwrapToStandardGroup
            let aUserGroupMember = aUser |> User.toUserGroupMember

            match aUserGroupMember with  
            | Ok aMember ->

                InternalRecursiveIsUserInNestedGroupService  aMember  loadGroupMemberById   aStandardGroup.Members
                   
            | Error error ->
                false

        
    
    let isGroupMemberIsInGroupServiceImpl: IsGroupMemberService = isGroupMemberIsInGroupServiceImplLocal loadGroupByGroupMemberIdDbDependencyFunction



    let groupMemberServices: GroupMemberServices = {
   
        TimeServiceWasCalled = DateTime.Now
        CallerCredentials = CallerCredential "FOTIO"
        isGroupMember = isGroupMemberIsInGroupServiceImpl
        isUserInNestedGroup = isUserInNestedGroupServiceImpl

    }

    

    
          

                


                                

///User type related services 
/// 
///                        
module User =



    ///Database dependecies 
     
    
    let loadUserByUserIdAndTenantIdDependencyFunction = UserDb.loadUserByUserIdAndTenantId


     
    










    let confirmUserServiveImpl : ConfirmUserServive = 

        fun loadUserByUserIdAndTenantId aGroup aUser ->

            let rsConfirmUser = result {


                let unWrappedGroup  =  aGroup |> unwrapToStandardGroup 
                let tenantIdFromUnWrappedGroup = unWrappedGroup.TenantId
                let userId = aUser.UserId

                let! confirmedUser = loadUserByUserIdAndTenantId userId tenantIdFromUnWrappedGroup


                     
                return confirmedUser
            }
            

            match rsConfirmUser with 
            | Ok confirmUser ->
                true
            | Error error ->
                false







