module IdentityAndAcccess.DomainServicesImplementations

open System
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Functions.ServiceInterfaces

open IdentityAndAcccess.EventStorePlayGround.Implementation
open IdentityAndAcccess.DomainTypes.Functions

open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Role
open FSharp.Data.Sql

























///Group type related services 
/// 
///  
module Group =





    ///Mongo Database dependecies 
    
    //let loadGroupByIdMongoDependencyFunction = GroupDb.loadOneGroupById

    //let loadGroupByGroupMemberIdDbDependencyFunction = GroupDb.loadOneGroupMemberById



    ///Gey Young Event Store Database dependecies 
    
    let loadGroupByIdGreyYoungEventStoreDependencyFunction = EventStorePlayGround.loadGroupWithGroupMemberId
    
    let loadGroupByGroupMemberIdGreyYoungEventStoreDependencyFunction = EventStorePlayGround.loadGroupWithGroupMemberId
     
                



    ///services
    
    
    

           


                    
            






    
                   

            
         

        
    
    

    
          

                









module Role =


    ()


    ///Database dependecies 
     
    


     
    







    //Authorisation relatedservices

    // let isUserInRoleServiceImpl : IsUserInRoleService = 

    //     fun  isUserInNestedGroupService confirmUserServive aRole aUser -> 

    //         match aUser.Enablement.EnablementStatus with 
    //         | Enabled -> 

    //             Role.isInRole isUserInNestedGroupService confirmUserServive aRole aUser

    //         | Disabled ->

    //             false


        
















                                

///User type related services 
/// 
///                        
module User =



    ///Database dependecies 
     
    


     
    










    //Domain related services
    let confirmUserServiveImpl : ConfirmUserServive = 

        fun aGroup aUser ->
            aUser.TenantId = aGroup.TenantId

            



    let passwordMatcherService : PasswordsMatcherService = 
        fun aPassword anEncryptedPasssword ->
            true





    // let allRolesForIdentifiedUser (isUserInNestedGroup:IsUserInNestedGroupService')  //Dependency
    //                               (confirmUserServive:ConfirmUserServive')          //Dependency
    //                               (allTenantRoles:Role list) (aTenant:Tenant) (aUser:User) : RoleDescriptor list = 

    //     let fromRoleToRoleDescriptor (aRole:Role) =
    //          {RoleId = aRole.RoleId; TenantId = aRole.TenantId; Name = aRole.Name}



    //     let isUserPlayingRole aUser aRole  = 
    //         Role.isInRole isUserInNestedGroup confirmUserServive aRole aUser

    //     let isUserPlayingRole' = isUserPlayingRole  aUser

    //     allTenantRoles
    //     |> List.filter isUserPlayingRole'
    //     |> List.map fromRoleToRoleDescriptor




    





