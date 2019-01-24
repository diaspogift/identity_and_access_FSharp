module IdentityAndAcccess.DomainServicesImplementations

open System
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation

open IdentityAndAcccess.EventStorePlayGround.Implementation
open IdentityAndAcccess.DomainTypes.Functions

open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAccess.DatabaseFunctionsInterfaceTypes
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Role
open FSharp.Data.Sql








let unwrapToStandardGroup aGroupToUnwrapp = 
        match aGroupToUnwrapp with 
            | Standard aStandardGroup -> aStandardGroup
            | Internal anInternalGroup -> anInternalGroup




///User type related services 
/// 
///  
module Tenant =


    ///Mongo Database dependecies 
    
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

            let! invitationDescription = "Invitation for Tenant ..."  |> RegistrationInvitationDescription.create' 

            let! tenantToProvision = Tenant.createFullActivatedTenant aTenantName aTenantDescription


            let! tenantWithRegistrationInvitation, registrationInvitation =  Tenant.offerRegistrationInvitation  tenantToProvision  invitationDescription


            let! password = "123456" |> Password.create' 

            let!  strongPassword = password |> strongPasswordService


            let! encryptedPassword = strongPassword |> passwordEncryptionService


            let! unwrappedEncryptedPassword = encryptedPassword 
                                           |> EncrytedPassword.value 
                                           |> Password.create'



            let! adminUsername = "Default Aministrator" |> Username.create' 

            let! adminUserEnablement = Enablement.fullCreate enablementSartDate enablementEndDate EnablementStatus.Enabled

            let! adminUser = Tenant.registerUserForTenant tenantWithRegistrationInvitation registrationInvitation.RegistrationInvitationId adminUsername  
                                                          unwrappedEncryptedPassword adminUserEnablement anEmailAddress aPostalAddress aPrimaryTelephone 
                                                          aSecondaryTelephone anAdministorFirstName anAdministorMiddleName anAdministorLastName 

                                        
            let! rstenant, invitation = Tenant.withdrawRegistrationInvitation tenantWithRegistrationInvitation registrationInvitation.RegistrationInvitationId


            let! adminRoleName = "SUPER_ADMINISTRATOR" |> RoleName.create'
            let! adminRoleDescription = "SUPER_ADMINISTRATOR is a role that have access to all tenant'rsources" |> RoleDescription.create'

            let! adminRole = Tenant.provisionRole rstenant adminRoleName adminRoleDescription

            let! resultAssignUserToSuperAdminRole = Role.assignUser adminRole adminUser

      
            //IO operation kept and the end ????


            return (tenantToProvision, adminUser, resultAssignUserToSuperAdminRole, tenantWithRegistrationInvitation.RegistrationInvitations)
          }



    let provisionTenantServiceImpl' = provisionTenantServiceImpl strongPasswordServiceImpl passwordEncryptionServiceImpl






















///Group type related services 
/// 
///  
module Group =





    ///Mongo Database dependecies 
    
    let loadGroupByIdMongoDependencyFunction = GroupDb.loadOneGroupById

    let loadGroupByGroupMemberIdDbDependencyFunction = GroupDb.loadOneGroupMemberById



    ///Gey Young Event Store Database dependecies 
    
    let loadGroupByIdGreyYoungEventStoreDependencyFunction = EventStorePlayGround.loadGroupWithGroupMemberId
    

   

     
                



    ///services
    
    
    let isGroupMemberIsInGroupServiceMongoImplLocal  =

        fun loadGroupMemberById  // Database dependency function
            aGroup 
            aMember -> 
                
            let aStandardGroup = aGroup |> DomainHelpers.unwrapToStandardGroup


                    
            let rec InternalRecursiveIsGroupMemberIsInGroupService 
                    (aMember : GroupMember) (loadGroupMemberById : LoadGroupMemberById) (aGroupMemberList : GroupMember list) =
                                                             
                    match aGroupMemberList with
                    | [] -> 
                        false
                    | head::tail ->
                        if (head.Type = GroupGroupMember) && (head = aMember) then true
                        else if (head.Type = GroupGroupMember) && not (head = aMember) then
                        
                            let additionalGroupToSearch = loadGroupMemberById head.MemberId
                       
                            match additionalGroupToSearch with
                            | Ok anAdditionalGroupToSearch ->

                                let aStandardAdditionalGroupToSearch = 
                                        anAdditionalGroupToSearch 
                                        |> DomainHelpers.unwrapToStandardGroup                                        
                                let newMembersToAppend = aStandardAdditionalGroupToSearch.Members
                                let allMembers = tail @ newMembersToAppend
                                let result = InternalRecursiveIsGroupMemberIsInGroupService aMember loadGroupMemberById allMembers

                                result

                            | Error _ -> false
                        else   
                            false 



            InternalRecursiveIsGroupMemberIsInGroupService  aMember  loadGroupMemberById   aStandardGroup.Members



    let isUserInNestedGroupServiceLocal : IsUserInNestedGroupService = 

        fun loadGroupMemberById  //Database dependency
            aGroup 
            aUser ->




            let rec InternalRecursiveIsUserInNestedGroupService (aMember : GroupMember)  
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


            
            let aStandardGroup = aGroup |> DomainHelpers.unwrapToStandardGroup
            let aUserGroupMember = aUser |> User.toUserGroupMember


            match aUserGroupMember with  
            | Ok aMember ->

                InternalRecursiveIsUserInNestedGroupService  aMember  loadGroupMemberById   aStandardGroup.Members
                   
            | Error error ->
                false

        
    let isUserInNestedGroupServiceImpl = 
            isUserInNestedGroupServiceLocal loadGroupByGroupMemberIdDbDependencyFunction


    let isGroupMemberIsInGroupServiceMongoImpl: IsGroupMemberService = 
            isGroupMemberIsInGroupServiceMongoImplLocal loadGroupByGroupMemberIdDbDependencyFunction


    let isGroupMemberIsInGroupServiceGreyYoungEventStoreImpl: IsGroupMemberService = 
            isGroupMemberIsInGroupServiceMongoImplLocal loadGroupByIdGreyYoungEventStoreDependencyFunction



    let groupMemberServices: GroupMemberServices = {
        TimeServiceWasCalled = DateTime.Now
        CallerCredentials = CallerCredential "FOTIO"
        isGroupMember = isGroupMemberIsInGroupServiceMongoImpl
        isUserInNestedGroup = isUserInNestedGroupServiceLocal
        }

    

    
          

                









module Role =





    ///Database dependecies 
     
    
    let loadUserByUserIdAndTenantIdDependencyFunction = UserDb.loadUserByUserIdAndTenantId


     
    







    //Authorisation relatedservices

    let isUserInRoleServiceImpl : IsUserInRoleService' = 

        fun  isUserInNestedGroupService confirmUserServive aRole aUser -> 

            match aUser.Enablement.EnablementStatus with 
            | Enabled -> 

                Role.isInRole isUserInNestedGroupService confirmUserServive aRole aUser

            | Disabled ->

                false


        
















                                

///User type related services 
/// 
///                        
module User =



    ///Database dependecies 
     
    
    let loadUserByUserIdAndTenantIdDependencyFunction = UserDb.loadUserByUserIdAndTenantId


     
    










    //Domain related services
    let confirmUserServiveImpl : ConfirmUserServive = 

        fun loadUserByUserIdAndTenantId aGroup aUser ->
            let unWrappedGroup  =  aGroup |> unwrapToStandardGroup 
            aUser.TenantId = unWrappedGroup.TenantId

            



    let passwordMatcherService : PasswordsMatcherService = 
        fun aPassword anEncryptedPasssword ->
            true





    let allRolesForIdentifiedUser (isUserInNestedGroup:IsUserInNestedGroupService')  //Dependency
                                  (confirmUserServive:ConfirmUserServive')          //Dependency
                                  (allTenantRoles:Role list) (aTenant:Tenant) (aUser:User) : RoleDescriptor list = 

        let fromRoleToRoleDescriptor (aRole:Role) =
             {RoleId = aRole.RoleId; TenantId = aRole.TenantId; Name = aRole.Name}



        let isUserPlayingRole aUser aRole  = 
            Role.isInRole isUserInNestedGroup confirmUserServive aRole aUser

        let isUserPlayingRole' = isUserPlayingRole  aUser

        allTenantRoles
        |> List.filter isUserPlayingRole'
        |> List.map fromRoleToRoleDescriptor




    let authenticate : AuthenticationService = 
        fun aPasswordMatcherService             // Dependency
            isUserInNestedGroupService          // Dependency
            aUser aTenant aPassword ->

            match aTenant.ActivationStatus with
            | ActivationStatus.Activated ->

                match aUser.Enablement.EnablementStatus with
                | EnablementStatus.Enabled ->


                    let rsPasswordComparison = result {

                        let strUserEncryptedPassWord  = aUser.Password |> Password.value 
                        let! userEncryptedPassWord = strUserEncryptedPassWord |> EncrytedPassword.create'

                        let areSamePasswords = aPasswordMatcherService aPassword userEncryptedPassWord
            

                        return areSamePasswords
                    }

                    match rsPasswordComparison with 
                    | Ok areSamePasswords ->


                            if areSamePasswords then 

                                let userDescriptor = aUser |> User.toUserDesriptor

                                userDescriptor

                            else

                                Error "Password does not match"

                    | Error error ->
                        Error error

                    

                

                | EnablementStatus.Disabled ->


                    let msg = "User enablement status is desabled"

                    Error msg


            | Deactivated ->

                let msg = "User tenant Activation status is deactivated"

                Error msg





    let authenticate' = authenticate passwordMatcherService
