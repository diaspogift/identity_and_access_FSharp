module IdentityAndAccess.DatabaseTypes.Functions

open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Functions
open IdentityAndAcccess.CommonDomainTypes.Functions

 
open MongoDB.Driver
open  MongoDB.Bson
open IdentityAndAcccess.CommonDomainTypes
open Suave.Sockets



let rec traverseResultA f list =

        // define the applicative functions
        let (<*>) = Result.apply
        let retn = Result.Ok

        // define a "cons" function
        let cons head tail = head :: tail

        // loop through the list
        match list with
        | [] -> 
            // if empty, lift [] to a Result
            retn []
        | head::tail ->
            // otherwise lift the head to a Result using f
            // and cons it with the lifted version of the remaining list
            retn cons <*> (f head) <*> (traverseResultA f tail)


module DbConfig =

    let connectionString = "mongodb://localhost"
    let client = new MongoClient(connectionString)
    let identityAndAccessDb = client.GetDatabase("IdentityAndAccessDb")

    let roleCollection = identityAndAccessDb.GetCollection<RoleDto> "roles"
    let userCollection = identityAndAccessDb.GetCollection<UserDto> "users"
    let groupCollection = identityAndAccessDb.GetCollection<GroupDto> "groups"
    let tenantCollection = identityAndAccessDb.GetCollection<TenantDto> "tenants"
     
 


module DbHelpers =

    let fromOneRegInvListToOneRegInvDtoList (aRegistrationInvitation : RegistrationInvitation) : RegistrationInvitationDto =



        printfn("---------------------------------------------------")

        printfn "IN fromOneRegInvListToOneRegInvDtoList and the value is : %A" (RegistrationInvitationId.value aRegistrationInvitation.RegistrationInvitationId)
        
        let id = new BsonObjectId(new ObjectId((RegistrationInvitationId.value aRegistrationInvitation.RegistrationInvitationId)))
        printfn("---------------------------------------------------")

        {
            RegistrationInvitationId = id
            Description = RegistrationInvitationDescription.value aRegistrationInvitation.Description
            TenantId = TenantId.value aRegistrationInvitation.TenantId
            StartingOn = aRegistrationInvitation.StartingOn
            Until = aRegistrationInvitation.Until
        }
    let fromRegInvListToRegInvDtoArray (aRegistrationInvitationList : RegistrationInvitation list) : RegistrationInvitationDto array =
        aRegistrationInvitationList 
        |> List.map fromOneRegInvListToOneRegInvDtoList
        |> List.toArray 



    let fromDbDtoToTenant (aDtoTenant : TenantDto) = 

        let id = aDtoTenant.TenantId.ToString()


        let fromAcitvationStatusDtoToAcitvationStatus (anAcitvationStatusDto : ActivationStatusDto)= 
            match anAcitvationStatusDto with 
            | ActivationStatusDto.Activated  -> Ok ActivationStatus.Activated
            | ActivationStatusDto.Disactivated -> Ok ActivationStatus.Disactivated
            | _ -> Error "Non matching case given"


         
        result {


            let! activationStatus = aDtoTenant.ActivationStatus |> fromAcitvationStatusDtoToAcitvationStatus

            let! tenant = Tenant.fullCreate id aDtoTenant.Name aDtoTenant.Description activationStatus
            return tenant
        }
    let fromTenantDomainToDto (aTenant:Tenant) = 


        let id = new BsonObjectId(new ObjectId((TenantId.value aTenant.TenantId)))

        let activationStatus = match aTenant.ActivationStatus  with 
                                | Activated  -> ActivationStatusDto.Activated
                                | Disactivated -> ActivationStatusDto.Disactivated
        let invitations = aTenant.RegistrationInvitations

        printfn "Look at this invitations : %A" invitations


        let invitationsDtos = invitations |> fromRegInvListToRegInvDtoArray 

        printfn "Look at this : %A" aTenant.RegistrationInvitations

        let res : TenantDto = {
            _id = id
            TenantId = id
            Name = TenantName.value aTenant.Name
            Description = TenantDescription.value aTenant.Description
            RegistrationInvitations = invitationsDtos
            ActivationStatus = activationStatus
        }

        res
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

        {
            _id = id
            UserId = id
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
            MiddleName = MiddleName.value aUser.Person.Name.Middle 
        }



    let fromGroupMemberToGroupMemberDto (aGroupMember : GroupMember) = 

        let memberType = match aGroupMember.Type  with 
                                    | Group  -> GroupMemberTypeDto.Group
                                    | User -> GroupMemberTypeDto.User


        let id = new BsonObjectId(new ObjectId((GroupMemberId.value aGroupMember.MemberId)))

        let res : GroupMemberDto = {
            MemberId =  id 
            TenantId = TenantId.value aGroupMember.TenantId
            Name = GroupMemberName.value aGroupMember.Name
            Type = memberType
        }

        res
    let fromGroupDomainToDto (aGroup:IdentityAndAcccess.DomainTypes.Group) = 

        let allGroupMembers = (aGroup.Members |> List.toArray )
        let id = new BsonObjectId(new ObjectId((GroupId.value aGroup.GroupId)))
        let res:GroupDto ={
            _id = id
            GroupId = id
            TenantId = TenantId.value aGroup.TenantId
            Name = GroupName.value aGroup.Name
            Description = GroupDescription.value aGroup.Description
            Members = (aGroup.Members |> List.toArray |> Array.map fromGroupMemberToGroupMemberDto)
        }

        res
    let fromRoleDomainToDto (aRole : Role) : RoleDto = 

        let id = new BsonObjectId(new ObjectId((RoleId.value aRole.RoleId)))

        let supportNestingStatus = match aRole.SupportNesting  with 
                                    | Support  -> SupportNestingStatusDto.Support
                                    | Oppose -> SupportNestingStatusDto.Oppose
                                    
        let groupDto = fromGroupDomainToDto  aRole.Group

        {
            _id = id
            RoleId = id
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


    let fromDbDtoToGroup (aDtoGroup : GroupDto) = 

        let id = aDtoGroup.GroupId.ToString()

        result {
            let! group = Group.create id aDtoGroup.TenantId aDtoGroup.Name aDtoGroup.Description []
            return group
        }
    
    
    
    
module RoleDb =

    let private saveRole (aRoleCollection : IMongoCollection<RoleDto>)(aRoleDto:RoleDto) = 
        aRoleCollection.InsertOne aRoleDto
    let private loadRoleById (aRoleCollection : IMongoCollection<RoleDto>)  ( id : BsonObjectId ) = 
        let result = aRoleCollection.Find(fun x -> x.RoleId = id).Single()        
        result

    let private updateRole (aRoleCollection : IMongoCollection<RoleDto>)  ( aRoleDto : RoleDto ) = 

        let filter = Builders<RoleDto>.Filter.Eq((fun x -> x.RoleId), aRoleDto.RoleId)
        let updateDefinition = Builders<RoleDto>.Update.Set((fun x -> x.Description), aRoleDto.Description).Set((fun x -> x.Name), aRoleDto.Name).Set((fun x -> x.SupportNesting), aRoleDto.SupportNesting).Set((fun x -> x.Group), aRoleDto.Group)
        let result = aRoleCollection.UpdateOne(filter, updateDefinition)
        ()
        









    let saveOneRole = saveRole DbConfig.roleCollection
    let loadOneRoleById: BsonObjectId -> RoleDto = loadRoleById DbConfig.roleCollection
    let updateOneRole: RoleDto -> unit = updateRole DbConfig.roleCollection






module UserDb =

    let private saveUser (aUserCollection : IMongoCollection<UserDto>)(aUserDto:UserDto) = 
        aUserCollection.InsertOne aUserDto
    let private loadUserById (aUserCollection : IMongoCollection<UserDto>)  ( id : BsonObjectId ) = 
        let result = aUserCollection.Find(fun x -> x.UserId = id).Single()        
        result

    let private updateUser (aUserCollection : IMongoCollection<UserDto>)  ( aUserDto : UserDto ) = 
        let filter = Builders<UserDto>.Filter.Eq((fun x -> x.UserId), aUserDto.UserId)
        let updateDefinition = Builders<UserDto>.Update.Set((fun x -> x.TenantId), aUserDto.TenantId).Set((fun x -> x.Username), aUserDto.Username).Set((fun x -> x.Password), aUserDto.Password)  .Set((fun x -> x.EnablementStatus), aUserDto.EnablementStatus)  .Set((fun x -> x.EmailAddress), aUserDto.EmailAddress)  .Set((fun x -> x.EnablementStartDate), aUserDto.EnablementStartDate)  .Set((fun x -> x.EnablementEndDate), aUserDto.EnablementEndDate)  .Set((fun x -> x.FirstName), aUserDto.FirstName).Set((fun x -> x.MiddleName), aUserDto.MiddleName).Set((fun x -> x.LastName), aUserDto.LastName).Set((fun x -> x.PostalAddress), aUserDto.PostalAddress).Set((fun x -> x.PrimaryTel), aUserDto.PrimaryTel).Set((fun x -> x.SecondaryTel), aUserDto.SecondaryTel)
        let result = aUserCollection.UpdateOne(filter, updateDefinition)
        ()
        









    let saveOneUser = saveUser DbConfig.userCollection
    let loadOneUserById: BsonObjectId -> UserDto = loadUserById DbConfig.userCollection
    let updateOneUser: UserDto -> unit = updateUser DbConfig.userCollection



module TenantDb =
    let private saveTenant (aTenantCollection : IMongoCollection<TenantDto>)(aTenantDto:TenantDto) = 
        aTenantCollection.InsertOne aTenantDto
    let private loadTenantById (aUserCollection : IMongoCollection<TenantDto>)  ( id : BsonObjectId ) = 
        printfn "Here in loadTenantById"
        let result = aUserCollection.Find(fun x -> x._id = id).Single()
        printfn "Here in loadTenantById the tenantDto is: %A" result
 
        result

    let private updateTenant (aTenantCollection : IMongoCollection<TenantDto>)  ( aTenantDto : TenantDto ) = 
        let filter = Builders<TenantDto>.Filter.Eq((fun x -> x.TenantId), aTenantDto.TenantId)
        let updateDefinition = Builders<TenantDto>.Update.Set((fun x -> x.Name), aTenantDto.Name).Set((fun x -> x.ActivationStatus), aTenantDto.ActivationStatus).Set((fun x -> x.Description), aTenantDto.Description).Set((fun x -> x.RegistrationInvitations), aTenantDto.RegistrationInvitations) 
        let result = aTenantCollection.UpdateOne(filter, updateDefinition)
        ()
        









    let saveOneTenant = saveTenant DbConfig.tenantCollection
    let loadOneTenantById: BsonObjectId -> TenantDto = loadTenantById DbConfig.tenantCollection
    let updateOneTenant: TenantDto -> unit = updateTenant DbConfig.tenantCollection



module GroupDb =

    let saveGroup (aGroupCollection : IMongoCollection<GroupDto>)  (aGroupDto:GroupDto) = 

        aGroupCollection.InsertOne aGroupDto

    let private loadGroupById (aGroupCollection : IMongoCollection<GroupDto>)  ( id : BsonObjectId ) = 
        let result = aGroupCollection.Find(fun x -> x.GroupId = id).Single()        
        result

    let private updateGroup (aGroupCollection : IMongoCollection<GroupDto>)  ( aGroupDto : GroupDto ) = 
        let filter = Builders<GroupDto>.Filter.Eq((fun x -> x.GroupId), aGroupDto.GroupId)
        let updateDefinition = Builders<GroupDto>.Update.Set((fun x -> x.TenantId), aGroupDto.TenantId).Set((fun x -> x.Name), aGroupDto.Name).Set((fun x -> x.Description), aGroupDto.Description)  .Set((fun x -> x.Members), aGroupDto.Members)  
        let result = aGroupCollection.UpdateOne(filter, updateDefinition)
        ()
        










    let saveOneGroup = saveGroup DbConfig.groupCollection
    let loadOneGroupById: BsonObjectId -> GroupDto = loadGroupById DbConfig.groupCollection
    let updateOneGroup: GroupDto -> unit = updateGroup DbConfig.groupCollection

