module IdentityAndAccess.DatabaseFunctionsInterfaceTypes.Implementation

open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.CommonDomainTypes.Functions
open IdentityAndAccess.DatabaseTypes
 

 
open MongoDB.Driver
open MongoDB.Bson
open FSharp.Data.Sql
open IdentityAndAcccess.DomainTypes








module DbConfig =

    let _DB_DEV_CONNECTION_URL_ = "mongodb://localhost"
    let _MONGO_DB_CLIENT_CONNECTIONT = new MongoClient(_DB_DEV_CONNECTION_URL_)
    let _INDENTITYY_AND_ACCESS_DB_ = _MONGO_DB_CLIENT_CONNECTIONT.GetDatabase("IdentityAndAccessDb")

    let roleCollection = "roles" |> _INDENTITYY_AND_ACCESS_DB_.GetCollection<RoleDto> 
    let userCollection = "users" |> _INDENTITYY_AND_ACCESS_DB_.GetCollection<UserDto> 
    let groupCollection = "groups" |> _INDENTITYY_AND_ACCESS_DB_.GetCollection<GroupDto> 
    let tenantCollection = "tenants" |> _INDENTITYY_AND_ACCESS_DB_.GetCollection<TenantDto> 
     
 


module DbHelpers =




    let fromOneRegInvListToOneRegInvDtoTempList 
        (aRegistrationInvitation : RegistrationInvitation) 
        : RegistrationInvitationDtoTemp =

        let srtRegInvId = aRegistrationInvitation.RegistrationInvitationId 
                          |>RegistrationInvitationId.value

        let id = new BsonObjectId(new ObjectId(srtRegInvId))
        {
            RegistrationInvitationId = id.ToString()
            Description = RegistrationInvitationDescription.value aRegistrationInvitation.Description
            TenantId = TenantId.value aRegistrationInvitation.TenantId
            StartingOn = aRegistrationInvitation.StartingOn
            Until = aRegistrationInvitation.Until
        }





    let fromRegInvListToRegInvDtoTempArray (aRegistrationInvitationList : RegistrationInvitation list) : RegistrationInvitationDtoTemp array =
        aRegistrationInvitationList 
        |> List.map fromOneRegInvListToOneRegInvDtoTempList
        |> List.toArray 





    let fromDbDtoToTenant (aDtoTenant : TenantDto) = 


        let id = aDtoTenant.TenantId.ToString()
        let fromAcitvationStatusDtoToAcitvationStatus (anAcitvationStatusDto : ActivationStatusDto)= 
            match anAcitvationStatusDto with 
            | ActivationStatusDto.Activated  -> Ok ActivationStatus.Activated
            | ActivationStatusDto.Disactivated -> Ok Deactivated
            | _ -> Error "Unconsistent state"
         
        result {
            let! activationStatus = aDtoTenant.ActivationStatus |> fromAcitvationStatusDtoToAcitvationStatus
            let! tenant = Tenant.fullCreate id aDtoTenant.Name aDtoTenant.Description activationStatus aDtoTenant.RegistrationInvitations
            return tenant
        }





    let fromTenantDomainToDto (aTenant:Tenant) = 

        let id = new BsonObjectId(new ObjectId((TenantId.value aTenant.TenantId)))
        let activationStatus = match aTenant.ActivationStatus  with 
                                | Activated  -> ActivationStatusDto.Activated
                                | Deactivated -> ActivationStatusDto.Disactivated
        let invitations = aTenant.RegistrationInvitations
        let invitationsDtos = invitations 
                              |> fromRegInvListToRegInvDtoTempArray 

        let rsTenantDto : TenantDto = {
            _id = id.ToString()
            TenantId = id.ToString()
            Name = TenantName.value aTenant.Name
            Description = TenantDescription.value aTenant.Description
            RegistrationInvitations = invitationsDtos
            ActivationStatus = activationStatus
        }

        rsTenantDto




        
    let fromDbDtoToUser (aDtoUser : UserDto) = 

        let id = aDtoUser.UserId.ToString()
        result {
            let! user = User.create id aDtoUser.TenantId aDtoUser.FirstName aDtoUser.MiddleName aDtoUser.LastName aDtoUser.EmailAddress aDtoUser.PostalAddress aDtoUser.PrimaryTel aDtoUser.SecondaryTel aDtoUser.Username aDtoUser.Password
            return user
        }

    let fromUserDomainToDto(aUser:User) = 

        let id = new BsonObjectId(new ObjectId((UserId.value aUser.UserId)))
        let enablementStatus = match aUser.Enablement.EnablementStatus  with 
                                | Enabled  -> EnablementStatusDto.Enabled
                                | Disabled -> EnablementStatusDto.Disabled

        let r:UserDto = {
            _id = id.ToString()
            UserId = id.ToString()
            TenantId = TenantId.value aUser.TenantId
            Username = Username.value aUser.Username
            Password = Password.value aUser.Password
            EnablementStatus = enablementStatus
            EnablementStartDate = aUser.Enablement.StartDate
            EnablementEndDate = aUser.Enablement.EndDate
            EmailAddress =  EmailAddress.value aUser.Person.Contact.Email
            PostalAddress = PostalAddress.value aUser.Person.Contact.Address
            PrimaryTel = Telephone.value aUser.Person.Contact.PrimaryTel
            SecondaryTel = Telephone.value aUser.Person.Contact.SecondaryTel
            FirstName = FirstName.value aUser.Person.Name.First
            LastName = LastName.value aUser.Person.Name.Last
            MiddleName = MiddleName.value  aUser.Person.Name.Middle
        }

        r







    let fromGroupMemberToGroupMemberDto (aGroupMember : GroupMember) = 

        let memberType = match aGroupMember.Type  with 
                         | GroupGroupMember  -> GroupMemberTypeDto.Group
                         | UserGroupMember -> GroupMemberTypeDto.User
        let grouMemberId = aGroupMember.MemberId
                           |> GroupMemberId.value
        let id = new BsonObjectId(new ObjectId(grouMemberId))

        let groupMemberToGroupMemberDto : GroupMemberDto = {
            MemberId =  id 
            TenantId = TenantId.value aGroupMember.TenantId
            Name = GroupMemberName.value aGroupMember.Name
            Type = memberType
        }

        groupMemberToGroupMemberDto





    let fromGroupDomainToDto (aGroup:Group) = 

        match aGroup with
              | Standard standardGroup -> 

                    let allGroupMembers = standardGroup.Members 
                                          |> List.toArray 
                                          |> Array.map fromGroupMemberToGroupMemberDto
                    let groupId = standardGroup.GroupId
                                  |> GroupId.value

                    printfn "I AM HERRRE AND GROUPID IS : ==== %A" standardGroup

                                  
                    let id = new BsonObjectId(new ObjectId(groupId))

                    let rsGroupDto:GroupDto ={
                        _id = id.ToString()
                        GroupId = id.ToString()
                        TenantId = TenantId.value standardGroup.TenantId
                        Name = GroupName.value standardGroup.Name
                        Description = GroupDescription.value standardGroup.Description
                        Members = allGroupMembers
                    }

                    rsGroupDto  

              | Internal internalGroup ->
                    let allGroupMembers = internalGroup.Members 
                                          |> List.toArray 
                                          |> Array.map fromGroupMemberToGroupMemberDto
                    let groupId = internalGroup.GroupId
                                  |> GroupId.value
                    let id = new BsonObjectId(new ObjectId(groupId))

                    let rsGroupDto:GroupDto ={
                        _id = id.ToString()
                        GroupId = id.ToString()
                        TenantId = TenantId.value internalGroup.TenantId
                        Name = GroupName.value internalGroup.Name
                        Description = GroupDescription.value internalGroup.Description
                        Members = allGroupMembers
                    }

                    rsGroupDto






    let fromRoleDomainToDto (aRole : Role) : RoleDto = 


        let roleId = aRole.RoleId |> RoleId.value
        let id = new BsonObjectId(new ObjectId(roleId))
        let supportNestingStatus = match aRole.SupportNesting  with 
                                   | Support  -> SupportNestingStatusDto.Support
                                   | Oppose -> SupportNestingStatusDto.Oppose
                                    
        let groupDto = fromGroupDomainToDto  aRole.InternalGroup

        {
            _id = id.ToString()
            RoleId = id.ToString()
            TenantId = TenantId.value aRole.TenantId
            Name = RoleName.value aRole.Name
            Description = RoleDescription.value aRole.Description
            SupportNesting = supportNestingStatus
            Group = groupDto 
        }






    let fromDtoToRoleDomain (aRoleDto : RoleDto) : Result<Role,string> = 

        let id = aRoleDto.RoleId.ToString()

        let role = result {
            let! role = Role.create id aRoleDto.TenantId aRoleDto.Name aRoleDto.Description
            return role
        }

        role





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



    let fromDbDtoToGroup (aGroupDtoToConvertToGroup : GroupDto) :Result<Group,string> = 

            let convertGrouMemberDtoToGroupMemberTempDto = 
                fun (groupMemberDto:GroupMemberDto) ->  

                                   let rsGroupMember = result {
                                        
                                        let strDtoGroupMemberId = groupMemberDto.MemberId.ToString()
                                        let! groupMemberIdFrom_strDtoGroupMemberId =  GroupMemberId.create "" strDtoGroupMemberId
                                        let! tenantId = TenantId.create "" groupMemberDto.TenantId 
                                        let! name = GroupMemberName.create "" groupMemberDto.Name 

                                        let grouMember:GroupMemberDtoTemp = {
                                            MemberId = GroupMemberId.value groupMemberIdFrom_strDtoGroupMemberId
                                            TenantId = TenantId.value tenantId
                                            Name = GroupMemberName.value name
                                            Type =  groupMemberDto.ToString() 
                                        }

                                       return grouMember

                                   } 

                                   rsGroupMember
            
            let id = aGroupDtoToConvertToGroup.GroupId.ToString()
            let groupMemberDtoListToConvertIntoGroupMemberDtoTempList = 
                   aGroupDtoToConvertToGroup.Members
                   |> Array.map convertGrouMemberDtoToGroupMemberTempDto
                   |> Array.toList
                   |> ResultOfSequenceTemp

            match groupMemberDtoListToConvertIntoGroupMemberDtoTempList with
            | Ok aGroupMemberDtoTempList -> 

                result {
                let! group = Group.create id aGroupDtoToConvertToGroup.TenantId aGroupDtoToConvertToGroup.Name aGroupDtoToConvertToGroup.Description aGroupMemberDtoTempList
                return group
                }

            |  Error error ->
                Error error                              
                           

        
    
    
    
    
module RoleDb =






    let private saveRole (aRoleCollection : IMongoCollection<RoleDto>)(aRoleDto:RoleDto) = 

        try
            aRoleDto 
            |>aRoleCollection.InsertOne 
            |> Ok
        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | _ -> Error "Unmatched error occurred"  




    let saveRoleAdapted (aGroupCollection : IMongoCollection<RoleDto>)  (aRole:Role) = 
               
            try   
                aRole 
                |> DbHelpers.fromRoleDomainToDto
                |> aGroupCollection.InsertOne 
                |> Ok
            with
                | :? System.TypeInitializationException as ex -> Error "er2"    
                | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
                | Failure msg -> Error "er0" 
                | e -> Error e.Message




    let private loadRoleById (aRoleCollection : IMongoCollection<RoleDto>)  ( id : BsonObjectId ) = 

        try

            aRoleCollection.Find(fun x -> x._id = id.ToString()).Single()            
            |> Ok
                
        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | _ -> Error "Unmatched error occurred"  



    let private loadRoleByRoleIdAndTenantId (aRoleCollection : IMongoCollection<RoleDto>)  ( roleId : BsonObjectId ) ( tenantId : BsonObjectId )  =

        try

            
            aRoleCollection.Find(fun x -> (x._id = roleId.ToString()) && (x.TenantId = tenantId.Value.ToString()) ).Single()
            |> Ok
                
        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | _ -> Error "Unmatched error occurred"  







    let private updateRole (aRoleCollection : IMongoCollection<RoleDto>)  ( aRoleDto : RoleDto ) = 

        try

            let filter = Builders<RoleDto>.Filter.Eq((fun x -> x.RoleId), aRoleDto.RoleId)
            let updateDefinition = Builders<RoleDto>.Update.Set((fun x -> x.Description), aRoleDto.Description).Set((fun x -> x.Name), aRoleDto.Name).Set((fun x -> x.SupportNesting), aRoleDto.SupportNesting).Set((fun x -> x.Group), aRoleDto.Group)
            
            let r = aRoleCollection.UpdateOne(filter, updateDefinition)
           
            Ok ()

        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | _ -> Error "Unmatched error occurred"  

        









    let saveOneRole : RoleDb.SaveOneRole = saveRoleAdapted DbConfig.roleCollection

    let loadOneRoleById : BsonObjectId -> Result<RoleDto,string> = loadRoleById DbConfig.roleCollection
    let updateOneRole = updateRole DbConfig.roleCollection




















module UserDb =









    let saveUser (aUserCollection : IMongoCollection<UserDto>)(aUserDto:UserDto) = 
    
        try 
        
            aUserCollection.InsertOne aUserDto
            |> Ok

        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | _ -> Error "Unmatched error occurred"  







    let saveRoleAdapted (aGroupCollection : IMongoCollection<UserDto>)  (aUser:User) = 
                       
                    try   
                        aUser 
                        |> DbHelpers.fromUserDomainToDto 
                        |> aGroupCollection.InsertOne 
                        |> Ok
                    with
                        | :? System.TypeInitializationException as ex -> Error "er2"    
                        | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
                        | Failure msg -> Error "er0" 
                        | e -> Error e.Message





    let loadUserById (aUserCollection : IMongoCollection<UserDto>)  ( id : BsonObjectId ) = 
        
        try
            aUserCollection.Find(fun x -> x._id = id.ToString()).Single()        
            |> Ok

        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | _ -> Error "Unmatched error occurred"  







    let loadUserByUserIdAndTenantId (aRoleCollection : IMongoCollection<UserDto>)  ( userId : BsonObjectId ) ( tenantId : BsonObjectId )  =

        try

            aRoleCollection.Find(fun x -> (x._id = userId.ToString()) && (x.TenantId = tenantId.Value.ToString()) ).Single()
            |> Ok
            
        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | _ -> Error "Unmatched error occurred"  






    let updateUser (aUserCollection : IMongoCollection<UserDto>)  ( aUserDto : UserDto ) = 
        
            
        try
            let filter = Builders<UserDto>.Filter.Eq((fun x -> x.UserId), aUserDto.UserId)
            let updateDefinition = Builders<UserDto>.Update.Set((fun x -> x.TenantId), aUserDto.TenantId).Set((fun x -> x.Username), aUserDto.Username).Set((fun x -> x.Password), aUserDto.Password)  .Set((fun x -> x.EnablementStatus), aUserDto.EnablementStatus)  .Set((fun x -> x.EmailAddress), aUserDto.EmailAddress)  .Set((fun x -> x.EnablementStartDate), aUserDto.EnablementStartDate)  .Set((fun x -> x.EnablementEndDate), aUserDto.EnablementEndDate)  .Set((fun x -> x.FirstName), aUserDto.FirstName).Set((fun x -> x.MiddleName), aUserDto.MiddleName).Set((fun x -> x.LastName), aUserDto.LastName).Set((fun x -> x.PostalAddress), aUserDto.PostalAddress).Set((fun x -> x.PrimaryTel), aUserDto.PrimaryTel).Set((fun x -> x.SecondaryTel), aUserDto.SecondaryTel)
                
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


    let loadOneUserById: BsonObjectId -> Result<UserDto, string> = 
        loadUserById DbConfig.userCollection


    let loadOneUserByUserIdAndTenantId: BsonObjectId -> BsonObjectId -> Result<UserDto, string> = 
        loadUserByUserIdAndTenantId DbConfig.userCollection


    let updateOneUser: UserDto -> Result<unit,string> = 
        updateUser DbConfig.userCollection












module TenantDb =









    let private saveTenant (aTenantCollection : IMongoCollection<TenantDto>)(aTenantDto:TenantDto) = 

        try

            aTenantDto 
            |> aTenantCollection.InsertOne  
            |> Ok

        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | Failure msg -> Error "er0" 
            | e -> Error e.Message        







    let saveTenantAdapted (aTenantCollection : IMongoCollection<TenantDto>)  (aTenant:Tenant) = 
               
        try   
            aTenant 
            |> DbHelpers.fromTenantDomainToDto
            |> aTenantCollection.InsertOne 
            |> Ok
        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1 vv"
            | Failure msg -> Error "er0" 
            | e -> Error e.Message






        
    let private loadTenantById (aUserCollection : IMongoCollection<TenantDto>)  ( id : BsonObjectId ) = 

        try

            let tenant = aUserCollection.Find(fun x -> x._id = id.ToString()).Single() 
            
            tenant
            |> DbHelpers.fromDbDtoToTenant

        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1 load"
            | Failure msg -> Error "er0" 
            | e -> Error e.Message






    let private loadTenantByIdAdapted (aUserCollection : IMongoCollection<TenantDto>)  ( aTenantId : TenantId ) = 


        printfn " loadTenantByIdAdapted CALLED WITH TENANTID = %A " aTenantId

        let srtTenantId = TenantId.value aTenantId

        printfn " srtTenantId =  %A " srtTenantId
        printfn " srtTenantId =  %A " srtTenantId
        printfn " srtTenantId =  %A " srtTenantId
        printfn " srtTenantId =  %A " srtTenantId


        let bsonId = new BsonObjectId (new ObjectId(srtTenantId))

        printfn " bsonId =  %A " bsonId
        printfn " bsonId =  %A " bsonId
        printfn " bsonId =  %A " bsonId
        printfn " bsonId =  %A " bsonId



        try

            let tenant = aUserCollection.Find(fun x -> x._id = bsonId.ToString()).Single() 


            printfn " tenant =  %A " tenant
            printfn " tenant =  %A " tenant
            printfn " tenant =  %A " tenant

            
            tenant
            |> DbHelpers.fromDbDtoToTenant

        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | Failure msg -> Error "er0" 
            | e -> Error e.Message







    let private updateTenant (aTenantCollection : IMongoCollection<TenantDto>)  ( aTenantDto : TenantDto ) = 

        try
            let filter = Builders<TenantDto>.Filter.Eq((fun x -> x.TenantId), aTenantDto.TenantId)
            let updateDefinition = Builders<TenantDto>.Update.Set((fun x -> x.Name), aTenantDto.Name).Set((fun x -> x.ActivationStatus), aTenantDto.ActivationStatus).Set((fun x -> x.Description), aTenantDto.Description).Set((fun x -> x.RegistrationInvitations), aTenantDto.RegistrationInvitations) 
            let result = aTenantCollection.UpdateOne(filter, updateDefinition)
            
            Ok ()

        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1 update"
            | Failure msg -> Error "er0" 
            | e -> Error e.Message       
        


    let private updateTenantAdapted (aTenantCollection : IMongoCollection<TenantDto>)  ( aTenant : Tenant ) = 


        try

            let aTenantDto = aTenant |> DbHelpers.fromTenantDomainToDto 

            let filter = Builders<TenantDto>.Filter.Eq((fun x -> x.TenantId), aTenantDto.TenantId)
            let updateDefinition = Builders<TenantDto>.Update.Set((fun x -> x.Name), aTenantDto.Name).Set((fun x -> x.ActivationStatus), aTenantDto.ActivationStatus).Set((fun x -> x.Description), aTenantDto.Description).Set((fun x -> x.RegistrationInvitations), aTenantDto.RegistrationInvitations) 
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


    let saveGroupAdapted (aGroupCollection : IMongoCollection<GroupDto>)  (aGroup:Group) = 
           
        try   
            aGroup 
            |> DbHelpers.fromGroupDomainToDto
            |> aGroupCollection.InsertOne 
            |> Ok
        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | Failure msg -> Error "er0" 
            | e -> Error e.Message



    let private loadGroupById (aGroupCollection : IMongoCollection<GroupDto>)  ( id : BsonObjectId ) = 
        
        try

        let result = aGroupCollection.Find(fun x -> x._id = id.ToString()).Single()        
        
        Ok ()
        
        with
            | :? System.FormatException as ex -> Error "error0"    
            | :? System.TypeInitializationException as ex -> Error "error1"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "error2"
            | Failure msg -> Error (msg + ": By ...Faillure msg..." )
            | e -> Error e.Message


    let private loadGroupByIdAdaptedToGroupId (aGroupCollection : IMongoCollection<GroupDto>)  ( aGroupId : GroupId ) = 
         
        

        try

            let bsonId = new BsonObjectId (new ObjectId(GroupId.value aGroupId))
            let result = aGroupCollection.Find(fun x -> x._id = bsonId.ToString()).Single()        
        
            result 
            |> DbHelpers.fromDbDtoToGroup
            
         with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | Failure msg -> Error "er0" 
            | e -> Error e.Source            



    let private loadGroupByIdAdaptedToGroupMembertId (aGroupCollection : IMongoCollection<GroupDto>)  ( aGroupMemberId : GroupMemberId ) = 
         
        try

            let bsonId = new BsonObjectId (new ObjectId(GroupMemberId.value aGroupMemberId))
            let result = aGroupCollection.Find(fun x -> x._id = bsonId.ToString()).Single()        
            
            result 
            |> DbHelpers.fromDbDtoToGroup

         with

            | Failure msg -> Error "er0" 
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | :? System.TypeInitializationException as ex -> Error "er2"
            | _ -> Error "Unmatched error occurred" 





    let private updateGroup (aGroupCollection : IMongoCollection<GroupDto>)  ( aGroupDto : GroupDto ) = 
        
        try
        
            let filter = Builders<GroupDto>.Filter.Eq((fun x -> x.GroupId), aGroupDto.GroupId)
            let updateDefinition = Builders<GroupDto>.Update.Set((fun x -> x.TenantId), aGroupDto.TenantId).Set((fun x -> x.Name), aGroupDto.Name).Set((fun x -> x.Description), aGroupDto.Description)  .Set((fun x -> x.Members), aGroupDto.Members)  
            let result = aGroupCollection.UpdateOne(filter, updateDefinition)
            
            
            Ok()
        
        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | Failure msg -> Error "er0" 
            | _ -> Error "Unmatched error occurred"



    let private updateGroupAdapted (aGroupCollection : IMongoCollection<GroupDto>)  ( aGroup : Group ) = 
        

        try
            let aStandardGroup = match aGroup with
                                       | Standard g -> g
                                       | Internal g -> g 

            let bsonId = new BsonObjectId (new ObjectId(GroupId.value aStandardGroup.GroupId))
            let filter = Builders<GroupDto>.Filter.Eq((fun x -> x._id), bsonId.ToString())
            let aGroupDto = DbHelpers.fromGroupDomainToDto aGroup
            let updateDefinition = Builders<GroupDto>.Update.Set((fun x -> x.TenantId), aGroupDto.TenantId).Set((fun x -> x.Name), aGroupDto.Name).Set((fun x -> x.Description), aGroupDto.Description)  .Set((fun x -> x.Members), aGroupDto.Members)  
            let result = aGroupCollection.UpdateOne(filter, updateDefinition)
          

            Ok()
        
        with
            | :? System.TypeInitializationException as ex -> Error "er2"    
            | :? MongoDB.Driver.MongoWriteException as ex -> Error "er1"
            | Failure msg -> Error "er0" 
            | _ -> Error "Unmatched error occurred"

        










    let saveOneGroup : GroupDb.SaveOneGroup = saveGroupAdapted DbConfig.groupCollection
    let loadOneGroupById : GroupDb.LoadOneGroupById = loadGroupByIdAdaptedToGroupId DbConfig.groupCollection
    let loadOneGroupMemberById : GroupDb.LoadOneGroupByGroupMemberId = loadGroupByIdAdaptedToGroupMembertId DbConfig.groupCollection
    let updateOneGroup : GroupDb.UpdateOneGroup = updateGroupAdapted DbConfig.groupCollection

    
