module IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation

open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess
open IdentityAndAcccess.CommonDomainTypes.Functions
 

 
open MongoDB.Driver
open MongoDB.Bson
open FSharp.Data.Sql
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.Functions.Dto
open IdentityAndAcccess.DomainTypes.Functions.Dto
open System.Text.RegularExpressions
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.DomainTypes.Group







module DbConfig =

    let connSting = "mongodb://localhost"
    let mongoClientConn = new MongoClient(connSting)
    let identityAndAccess = mongoClientConn.GetDatabase("IdentityAndAccessDb")

    let roleCollection = "roles" |> identityAndAccess.GetCollection<Dto.Role> 
    let userCollection = "users" |> identityAndAccess.GetCollection<Dto.User> 
    let groupCollection = "groups" |> identityAndAccess.GetCollection<Dto.Group> 
    let tenantCollection = "tenants" |> identityAndAccess.GetCollection<Dto.Tenant> 
     
 


module DbHelpers =




    // let fromOneRegInvListToOneRegInvDtoTempList 
    //     (aRegistrationInvitation : RegistrationInvitation) 
    //     : RegistrationInvitationDtoTemp =

    //     let srtRegInvId = aRegistrationInvitation.RegistrationInvitationId 
    //                       |>RegistrationInvitationId.value

    //     let id = new BsonObjectId(new ObjectId(srtRegInvId))
    //     {
    //         RegistrationInvitationId = id.ToString()
    //         Description = RegistrationInvitationDescription.value aRegistrationInvitation.Description
    //         TenantId = TenantId.value aRegistrationInvitation.TenantId
    //         StartingOn = aRegistrationInvitation.StartingOn
    //         Until = aRegistrationInvitation.Until
    //     }





    // let fromRegInvListToRegInvDtoTempArray (aRegistrationInvitationList : RegistrationInvitation list) : RegistrationInvitationDtoTemp array =
    //     aRegistrationInvitationList 
    //     |> List.map fromOneRegInvListToOneRegInvDtoTempList
    //     |> List.toArray 





    // let fromDbDtoToTenant (aDtoTenant : TenantDto) = 


    //     let id = aDtoTenant.TenantId.ToString()
    //     let fromAcitvationStatusDtoToAcitvationStatus (anAcitvationStatusDto : ActivationStatusDto)= 
    //         match anAcitvationStatusDto with 
    //         | ActivationStatusDto.Activated  -> Ok ActivationStatus.Activated
    //         | ActivationStatusDto.Disactivated -> Ok Deactivated
    //         | _ -> Error "Unconsistent state"
         
    //     result {
    //         let! activationStatus = aDtoTenant.ActivationStatus |> fromAcitvationStatusDtoToAcitvationStatus
    //         let! tenant = Tenant.fullCreate id aDtoTenant.Name aDtoTenant.Description activationStatus aDtoTenant.RegistrationInvitations
    //         return tenant
    //     }





    // let fromTenantDomainToDto (aTenant:Tenant) = 

    //     let id = new BsonObjectId(new ObjectId((TenantId.value aTenant.TenantId)))
    //     let activationStatus = match aTenant.ActivationStatus  with 
    //                             | Activated  -> ActivationStatusDto.Activated
    //                             | Deactivated -> ActivationStatusDto.Disactivated
    //     let invitations = aTenant.RegistrationInvitations
    //     let invitationsDtos = invitations 
    //                           |> fromRegInvListToRegInvDtoTempArray 

    //     let rsTenantDto : TenantDto = {
    //         _id = id.ToString()
    //         TenantId = id.ToString()
    //         Name = TenantName.value aTenant.Name
    //         Description = TenantDescription.value aTenant.Description
    //         RegistrationInvitations = invitationsDtos
    //         ActivationStatus = activationStatus
    //     }

    //     rsTenantDto




        
    // let fromDbDtoToUser (aDtoUser : UserDto) = 

    //     let id = aDtoUser.UserId.ToString()
    //     result {
    //         let! user = User.create id aDtoUser.TenantId aDtoUser.FirstName aDtoUser.MiddleName aDtoUser.LastName aDtoUser.EmailAddress aDtoUser.PostalAddress aDtoUser.PrimaryTel aDtoUser.SecondaryTel aDtoUser.Username aDtoUser.Password
    //         return user
    //     }

    // let fromUserDomainToDto(aUser:User) = 

    //     let id = new BsonObjectId(new ObjectId((UserId.value aUser.UserId)))
    //     let enablementStatus:EnablementStatusDto = match aUser.Enablement.EnablementStatus  with 
    //                                                | EnablementStatus.Enabled  -> EnablementStatusDto.Enabled
    //                                                | EnablementStatus.Disabled -> EnablementStatusDto.Disabled

    //     let r:UserDto = {
    //         _id = id.ToString()
    //         UserId = id.ToString()
    //         TenantId = TenantId.value aUser.TenantId
    //         Username = Username.value aUser.Username
    //         Password = Password.value aUser.Password
    //         EnablementStatus = enablementStatus
    //         EnablementStartDate = aUser.Enablement.StartDate
    //         EnablementEndDate = aUser.Enablement.EndDate
    //         EmailAddress =  EmailAddress.value aUser.Person.Contact.Email
    //         PostalAddress = PostalAddress.value aUser.Person.Contact.Address
    //         PrimaryTel = Telephone.value aUser.Person.Contact.PrimaryTel
    //         SecondaryTel = Telephone.value aUser.Person.Contact.SecondaryTel
    //         FirstName = FirstName.value aUser.Person.Name.First
    //         LastName = LastName.value aUser.Person.Name.Last
    //         MiddleName = MiddleName.value  aUser.Person.Name.Middle
    //     }

    //     r







    // let fromGroupMemberToGroupMemberDto (aGroupMember : GroupMember) = 

    //     let memberType = match aGroupMember.Type  with 
    //                      | GroupGroupMember  -> GroupMemberTypeDto.Group
    //                      | UserGroupMember -> GroupMemberTypeDto.User
    //     let grouMemberId = aGroupMember.MemberId
    //                        |> GroupMemberId.value

    //     let groupMemberToGroupMemberDto : GroupMemberDto = {
    //         MemberId =  grouMemberId
    //         TenantId = TenantId.value aGroupMember.TenantId
    //         Name = GroupMemberName.value aGroupMember.Name
    //         Type = memberType
    //     }

    //     groupMemberToGroupMemberDto





    // let fromGroupDomainToDto (aGroup:Group) = 

    //     match aGroup with
    //           | Standard standardGroup -> 

    //                 let allGroupMembers = 
    //                         standardGroup.Members 
    //                         |> List.toArray 
    //                         |> Array.map fromGroupMemberToGroupMemberDto
    //                 let groupId = standardGroup.GroupId |> GroupId.value          
    //                 let id = new BsonObjectId(new ObjectId(groupId))
    //                 let rsGroupDto:GroupDto ={
    //                     _id = id.ToString()
    //                     GroupId = id.ToString()
    //                     TenantId = TenantId.value standardGroup.TenantId
    //                     Name = GroupName.value standardGroup.Name
    //                     Description = GroupDescription.value standardGroup.Description
    //                     Members = allGroupMembers
    //                     }

    //                 rsGroupDto  

    //           | Internal internalGroup ->
    //                 let allGroupMembers = 
    //                     internalGroup.Members 
    //                     |> List.toArray 
    //                     |> Array.map fromGroupMemberToGroupMemberDto
    //                 let groupId = internalGroup.GroupId |> GroupId.value
    //                 let rsGroupDto:GroupDto ={
    //                     _id = groupId
    //                     GroupId = id.ToString()
    //                     TenantId = TenantId.value internalGroup.TenantId
    //                     Name = GroupName.value internalGroup.Name
    //                     Description = GroupDescription.value internalGroup.Description
    //                     Members = allGroupMembers
    //                     }

    //                 rsGroupDto






    // let fromRoleDomainToDto (aRole : Role) : RoleDto = 


    //     let roleId = aRole.RoleId |> RoleId.value
    //     let id = new BsonObjectId(new ObjectId(roleId))
    //     let supportNestingStatus = match aRole.SupportNesting  with 
    //                                | Support  -> SupportNestingStatusDto.Support
    //                                | Oppose -> SupportNestingStatusDto.Oppose
                                    
    //     let groupDto = fromGroupDomainToDto  aRole.InternalGroup

    //     {
    //         _id = id.ToString()
    //         RoleId = id.ToString()
    //         TenantId = TenantId.value aRole.TenantId
    //         Name = RoleName.value aRole.Name
    //         Description = RoleDescription.value aRole.Description
    //         SupportNesting = supportNestingStatus
    //         Group = groupDto 
    //     }






    // let fromDtoToRoleDomain (aRoleDto : RoleDto) : Result<Role,string> = 

    //     let id = aRoleDto.RoleId.ToString()

    //     let role = result {
    //         let! role = Role.create id aRoleDto.TenantId aRoleDto.Name aRoleDto.Description
    //         return role
    //     }

    //     role





///Sdould be in the common shared libs START
/// 
/// 
/// 
/// 
    let preppend  firstR restR = 
        match firstR, restR with
        | Ok first, Ok rest -> Ok (first::rest)  
        | Error error1, Ok _ -> Error error1
        | Ok _, Error error2 -> Error error2  
        | Error error1, Error _ -> Error error1






    let ResultOfSequenceTemp aListOfResults =
        let initialValue = Ok List.empty
        List.foldBack preppend aListOfResults initialValue


///Should be in the common shared libs END
/// 
/// 
/// 
/// 



    

        
    
    
    
    
module RoleDb =






    let private saveRole (aRoleCollection : IMongoCollection<Dto.Role>)(aRoleDto:Dto.Role) = 

        try
            aRoleDto 
            |> aRoleCollection.InsertOne 
            |> Ok
        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | _ -> Error "Unmatched error occurred"  


    let saveRoleAdapted (aGroupCollection : IMongoCollection<Dto.Role>)  (aRole:Role.Role) = 
               
            try   
                aRole 
                |> Role.fromDomain
                |> aGroupCollection.InsertOne 
                |> Ok
            with
                | :? System.TypeInitializationException as ex -> Error "er2"    
                | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
                | Failure msg -> Error "er0" 
                | e -> Error e.Message




    let private loadRoleById (aRoleCollection : IMongoCollection<Dto.Role>)  ( id : BsonObjectId ) = 

        try

            aRoleCollection.Find(fun x -> x.RoleId = id.ToString()).Single()            
            |> Ok
                
        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | _ -> Error "Unmatched error occurred"  



    let private loadRoleByRoleIdAndTenantId 
        (aRoleCollection : IMongoCollection<Dto.Role>)( roleId : BsonObjectId )( tenantId : BsonObjectId ):Result<Role.Role, string> =

        try

            
            aRoleCollection.Find(fun x -> (x.RoleId = roleId.ToString()) && (x.TenantId = tenantId.Value.ToString()) ).Single()
            |> Role.toDomain
            
                
        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | _ -> Error "Unmatched error occurred"  







    let private updateRole (aRoleCollection : IMongoCollection<Dto.Role>)  ( aRoleDto : Dto.Role ) = 

        try

            let filter = Builders<Dto.Role>.Filter.Eq((fun x -> x.RoleId), aRoleDto.RoleId)
            let updateDefinition = Builders<Dto.Role>.Update.Set((fun x -> x.Description), aRoleDto.Description).Set((fun x -> x.Name), aRoleDto.Name).Set((fun x -> x.SupportNesting), aRoleDto.SupportNesting).Set((fun x -> x.InternalGroup), aRoleDto.InternalGroup)
            
            let r = aRoleCollection.UpdateOne(filter, updateDefinition)
           
            Ok ()

        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | _ -> Error "Unmatched error occurred"  

        









    let saveOneRole : RoleDb.SaveOneRole = saveRoleAdapted DbConfig.roleCollection

    let loadOneRoleById : BsonObjectId -> Result<Dto.Role,string> = loadRoleById DbConfig.roleCollection
    let updateOneRole = updateRole DbConfig.roleCollection




















module UserDb =









    let saveUser (aUserCollection : IMongoCollection<Dto.User>)(aUserDto:Dto.User) = 
    
        try 
        
            aUserCollection.InsertOne aUserDto
            |> Ok

        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | _ -> Error "Unmatched error occurred"  







    let saveRoleAdapted (aGroupCollection : IMongoCollection<Dto.User>)  (aUser:User.User) = 
                       
                    try   
                        aUser 
                        |> User.fromDomain
                        |> aGroupCollection.InsertOne 
                        |> Ok
                    with
                        | :? System.TypeInitializationException as ex -> Error "er2"    
                        | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
                        | Failure msg -> Error "er0" 
                        | e -> Error e.Message





    let loadUserById (aUserCollection : IMongoCollection<Dto.User>)  ( id : BsonObjectId ) = 
        
        try
            aUserCollection.Find(fun x -> x.UserId = id.ToString()).Single()        
            |> Ok

        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | _ -> Error "Unmatched error occurred"  







    let loadUserByUserIdAndTenantId (aRoleCollection : IMongoCollection<Dto.User>)  ( userId : BsonObjectId ) ( tenantId : BsonObjectId )   =

        try

            aRoleCollection.Find(fun x -> (x.UserId = userId.ToString()) && (x.TenantId = tenantId.Value.ToString()) ).Single()
            |> Ok
            
        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | _ -> Error "Unmatched error occurred"  






    let updateUser (aUserCollection : IMongoCollection<Dto.User>)  ( aUserDto : Dto.User) = 
        
            
        try
            let filter = Builders<Dto.User>.Filter.Eq((fun x -> x.UserId), aUserDto.UserId)
            let updateDefinition = 
                Builders<Dto.User>.Update.Set((fun x -> x.TenantId), aUserDto.TenantId)
                    .Set((fun x -> x.Username), aUserDto.Username)
                    .Set((fun x -> x.Password), aUserDto.Password)
                    .Set((fun x -> x.Enablement), aUserDto.Enablement)  
                    .Set((fun x -> x.Person), aUserDto.Person)
                    .Set((fun x -> x.Enablement), aUserDto.Enablement)

          
            let r = aUserCollection.UpdateOne(filter, updateDefinition)
            ()
            |> Ok
            
        with
            //| :? System.FormatDigitExection as ex -> Error "er2"    
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | Failure msg -> Error "er0" 
            | _ -> Error "Unmatched error occurred" 




    let saveOneUser: UserDb.SaveOneUser = saveRoleAdapted DbConfig.userCollection


    let loadOneUserById: BsonObjectId -> Result<Dto.User, string> = 
        loadUserById DbConfig.userCollection


    let loadOneUserByUserIdAndTenantId: BsonObjectId -> BsonObjectId -> Result<Dto.User, string> = 
        loadUserByUserIdAndTenantId DbConfig.userCollection


    let updateOneUser: Dto.User -> Result<unit,string> = 
        updateUser DbConfig.userCollection












module TenantDb =









    let private saveTenant (aTenantCollection : IMongoCollection<Dto.Tenant>)(aTenantDto:Dto.Tenant) = 

        try

            aTenantDto 
            |> aTenantCollection.InsertOne  
            |> Ok

        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | Failure msg -> Error "er0" 
            | e -> Error e.Message        







    let saveTenantAdapted (aTenantCollection : IMongoCollection<Dto.Tenant>)  (aTenant:Tenant.Tenant) = 
               
        try   
            aTenant 
            |> Tenant.fromDomain
            |> aTenantCollection.InsertOne 
            |> Ok
        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1 vv"
            | Failure msg -> Error "er0" 
            | e -> Error e.Message






        
    let private loadTenantById (aUserCollection : IMongoCollection<Dto.Tenant>)  ( id : BsonObjectId ) = 

        try

            let tenant = aUserCollection.Find(fun x -> x.TenantId = id.ToString()).Single() 
            
            tenant
            |> Tenant.toDomain

        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1 load"
            | Failure msg -> Error "er0" 
            | e -> Error e.Message






    let private loadTenantByIdAdapted (aUserCollection : IMongoCollection<Dto.Tenant>)  ( aTenantId : CommonDomainTypes.TenantId ) = 

        let srtTenantId = TenantId.value aTenantId

        printfn " srtTenantId =  %A " srtTenantId


        let bsonId = new BsonObjectId (new ObjectId(srtTenantId))
        try

            let tenant = aUserCollection.Find(fun x -> x.TenantId = bsonId.ToString()).Single() 
            printfn " tenant =  %A " tenant
            tenant
            |> Tenant.toDomain

        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | Failure msg -> Error "er0" 
            | e -> Error e.Message







    let private updateTenant (aTenantCollection : IMongoCollection<Dto.Tenant>)  ( aTenantDto : Dto.Tenant ) = 

        try
            let filter = Builders<Dto.Tenant>.Filter.Eq((fun x -> x.TenantId), aTenantDto.TenantId)
            let updateDefinition = 
                Builders<Dto.Tenant>.Update
                    .Set((fun x -> x.Name), aTenantDto.Name)
                    .Set((fun x -> x.ActivationStatus), aTenantDto.ActivationStatus)
                    .Set((fun x -> x.Description), aTenantDto.Description)
                    .Set((fun x -> x.RegistrationInvitations), aTenantDto.RegistrationInvitations) 

            let result = aTenantCollection.UpdateOne(filter, updateDefinition)
            Ok ()

        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1 update"
            | Failure msg -> Error "er0" 
            | e -> Error e.Message       
        


    let private updateTenantAdapted (aTenantCollection : IMongoCollection<Dto.Tenant>)  ( aTenant : Tenant.Tenant ) = 


        try

            let aTenantDto = aTenant |> Tenant.fromDomain

            let filter = Builders<Dto.Tenant>.Filter.Eq((fun x -> x.TenantId), aTenantDto.TenantId)

            let updateDefinition = 
                Builders<Dto.Tenant>.Update
                    .Set((fun x -> x.Name), aTenantDto.Name)
                    .Set((fun x -> x.ActivationStatus), aTenantDto.ActivationStatus)
                    .Set((fun x -> x.Description), aTenantDto.Description)
                    .Set((fun x -> x.RegistrationInvitations), aTenantDto.RegistrationInvitations) 

            let result = aTenantCollection.UpdateOne(filter, updateDefinition)
            
            Ok ()

        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1 update"
            | Failure msg -> Error "er0" 
            | e -> Error e.Message       
              



    let saveOneTenant: TenantDb.SaveOneTenant = saveTenantAdapted DbConfig.tenantCollection
    let loadOneTenantById: TenantDb.LoadOneTenantById = loadTenantByIdAdapted DbConfig.tenantCollection
    let updateOneTenant: TenantDb.UpdateOneTenant = updateTenantAdapted DbConfig.tenantCollection









module GroupDb =






    let saveGroup (aGroupCollection : IMongoCollection<GroupDto>)  (aGroupDto:GroupDto) = 
         


        try

            aGroupDto
            |> aGroupCollection.InsertOne
            |> Ok

        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | Failure msg -> Error "er0" 
            | e -> Error e.Message


    let saveGroupAdapted (aGroupCollection : IMongoCollection<Dto.Group>)  (aGroup:Group.Group) = 
           
        try   
            aGroup 
            |> Group.fromDomain
            |> aGroupCollection.InsertOne 
            |> Ok
        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | Failure msg -> Error "er0" 
            | e -> Error e.Message



    let  loadGroupById (aGroupCollection : IMongoCollection<GroupDto>)  ( id : BsonObjectId ) = 
        
        try

        let result = aGroupCollection.Find(fun x -> x._id = id.ToString()).Single()        
        
        Ok ()
        
        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | e -> Error e.Message


    let private loadGroupByIdAdaptedToGroupId (aGroupCollection : IMongoCollection<Dto.StandardGroup>) ( aGroupId : CommonDomainTypes.GroupId ) = 
         
        

        try

            let strdDtoGrp = aGroupCollection.Find(fun x -> x.GroupId = GroupId.value aGroupId).Single() 
            let group = Dto.Group.Standard strdDtoGrp
        
            group
            |> Group.toDomain 
            
         with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | Failure msg -> Error "er0" 
            | e -> Error e.Source            



    let private loadGroupByIdAdaptedToGroupMembertId (aGroupCollection : IMongoCollection<Dto.StandardGroup>)  ( aGroupMemberId : CommonDomainTypes.GroupMemberId ) = 
         
        try

            let bsonId = new BsonObjectId (new ObjectId(GroupMemberId.value aGroupMemberId))
            let strdDtoGrp = aGroupCollection.Find(fun x -> x.GroupId = bsonId.ToString()).Single()        
            let group = Dto.Group.Standard strdDtoGrp

            group
            |> Group.toDomain

         with

            | Failure msg -> Error "er0" 
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | :? System.TypeInitializationException as ex -> Error "er2"
            | _ -> Error "Unmatched error occurred" 





    let private updateGroup (aGroupCollection : IMongoCollection<GroupDto>)  ( aGroupDto : GroupDto ) = 
        
        try
            let filter = Builders<GroupDto>.Filter.Eq((fun x -> x.GroupId), aGroupDto.GroupId)
            let updateDefinition = 
                Builders<GroupDto>.Update
                    .Set((fun x -> x.TenantId), aGroupDto.TenantId)
                    .Set((fun x -> x.Name), aGroupDto.Name)
                    .Set((fun x -> x.Description), aGroupDto.Description)  
                    .Set((fun x -> x.Members), aGroupDto.Members)  
                    
            let result = aGroupCollection.UpdateOne(filter, updateDefinition)    
            Ok()
        
        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | Failure msg -> Error "er0" 
            | _ -> Error "Unmatched error occurred"



    let private updateGroupAdapted (aGroupCollection : IMongoCollection<Dto.StandardGroup>)  ( aGroup : Group.Group ) = 
        

        try
            
            let aGroupDto = aGroup |> Group.fromDomain

            let aStandardGroupDto = match aGroupDto with
                                    | Dto.Group.Standard s ->  s
                                    | Dto.Group.Internal i -> i

            let filter = Builders<Dto.StandardGroup>.Filter.Eq((fun x -> x.GroupId), aStandardGroupDto.GroupId)

            let updateDefinition = 
                Builders<Dto.StandardGroup>.Update
                    .Set((fun x -> x.TenantId), aStandardGroupDto.TenantId)
                    .Set((fun x -> x.Name), aStandardGroupDto.Name)
                    .Set((fun x -> x.Description), aStandardGroupDto.Description)
                    .Set((fun x -> x.Members), aStandardGroupDto.Members)  

                    
            let result = aGroupCollection.UpdateOne(filter, updateDefinition)
          

            Ok()
        
        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | Failure msg -> Error "er0" 
            | _ -> Error "Unmatched error occurred"

        










    let saveOneGroup : GroupDb.SaveOneGroup = saveGroupAdapted DbConfig.groupCollection
    //let loadOneGroupById  = loadGroupByIdAdaptedToGroupId DbConfig.groupCollection
    //let loadOneGroupMemberById : GroupDb.LoadOneGroupByGroupMemberId = loadGroupByIdAdaptedToGroupMembertId DbConfig.groupCollection
    //let updateOneGroup : GroupDb.UpdateOneGroup = updateGroupAdapted DbConfig.groupCollection

    
