namespace IdentityAndAcccess.DomainTypes


///Imported local libs
open IdentityAndAcccess.CommonDomainTypes
open System
open IdentityAndAcccess.CommonDomainTypes.Functions
open System.Text.RegularExpressions
open Suave.Logging
open Suave.Logging







[<AutoOpen>]
module Events =





    ///Useful types
    /// 
    /// 
    /// 
    /// 
    /// 
    type DomainEvent<'EventData> = {

        EventVersion : int
        OccurredOn : DateTime
        Data : 'EventData

    }






































module User =







    ///User related types
    /// 
    /// 
    /// 
    /// 
    /// 
    type FullName = {
        First: FirstName
        Middle: MiddleName 
        Last: LastName
        }


    type EnablementStatus = 
        |Enabled 
        |Disabled


    type Enablement = {
        EnablementStatus: EnablementStatus
        StartDate: DateTime
        EndDate: DateTime
        }



    type ContactInformation = {
        Email : EmailAddress
        Address: PostalAddress
        PrimaryTel: Telephone
        SecondaryTel: Telephone
        }



    

    type ContactInformationChanged = 

        DomainEvent<ContactInformationChangedEventData>

    and ContactInformationChangedEventData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        FirstName: string
        LastName: string
        ContactInformation : string
        }


    type Person =  {
        Contact: ContactInformation
        Name: FullName 
        Tenant: TenantId
        User: UserId
        }


    



    type PersonalNameChanged = 

        DomainEvent<PersonalNameChangedEventData>

    and PersonalNameChangedEventData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        FirstName: string
        LastName: string
        }


    type User = {
        UserId: UserId
        TenantId: TenantId
        Username: Username
        Password: Password
        Enablement: Enablement
        Person: Person
        }

    type UserDescriptor = {
        UserDescriptorId: UserDescriptorId
        TenantId: TenantId
        Username: Username
        Email: EmailAddress
        }


    

    type UserEnablementChanged =

            DomainEvent<UserEnablementChangedEventData>

    and UserEnablementChangedEventData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        }




    



    type UserPasswordChanged = 

            DomainEvent<UserPasswordChangedEventData>

    and UserPasswordChangedEventData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        }


    
    
 


    type UserRegisteredChanged = 

        DomainEvent<UserRegisteredChangedEventData>

    and UserRegisteredChangedEventData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        FirstName: string
        LastName: string
        }












module Group =






    ///Group related types
    /// 
    /// 
    /// 
    /// 
    /// 
    type GroupMemberType = 
        | UserGroupMember
        | GroupGroupMember


    type GroupMember = {
        MemberId: GroupMemberId
        TenantId: TenantId
        Name: GroupMemberName
        Type: GroupMemberType
        }


    type StandardGroup = {
        GroupId: GroupId
        TenantId: TenantId
        Name: GroupName
        Description: GroupDescription
        Members: GroupMember list
        }



    type Group = 
        | Standard of StandardGroup
        | Internal of StandardGroup



     



    type GroupGroupAdded = 

            DomainEvent<GroupGroupAddedEventData>

    and GroupGroupAddedEventData = {

        GroupId: string
        NestedGroupId: string
        TenantId: string

    }

    



    type GroupGroupRemoved = 

        DomainEvent<GroupGroupRemovedEventData>
    
    and GroupGroupRemovedEventData = {

        GroupId: string
        RemovedGroupId: string
        TenantId: string

    }




 


    type GroupProvisioned = 

        DomainEvent<GroupProvisionedEventData>

    and GroupProvisionedEventData = {
        GroupId : string
        TenantId :string
        GroupName :string 
        }




    type GroupIdOrGroupMemberId = 
        | GroupId of GroupId
        | GroupMemberId of GroupMemberId



    ///Temporary types should there be here? let alone should they have been created?
    type GroupMemberDtoTemp = {
        MemberId : string
        TenantId : string
        Name : string
        Type : string
        }


















module Role =


    open Group




    ///Role related types
    /// 
    /// 
    /// 
    /// 
    type SupportNestingStatus = 
        | Support
        | Oppose

    type Role = {
        RoleId: RoleId
        TenantId: TenantId
        Name: RoleName
        Description: RoleDescription
        SupportNesting: SupportNestingStatus
        InternalGroup: Group
        }







    type RoleProvisioned = 

        DomainEvent<RoleProvisionedEventData>

    and RoleProvisionedEventData = {
        RoleId : string
        TenantId : string;
    }



    type GroupAssignedToRole = 

        DomainEvent<GroupAssignedToRoleEventData>

    and GroupAssignedToRoleEventData = {
        GroupId : string
        RoleId : string
        TenantId : string;
    }




    type GroupUnAssignedToRole = 

        DomainEvent<GroupUnAssignedToRoleEventData>

    and GroupUnAssignedToRoleEventData = {
        GroupId : string
        RoleId : string
        TenantId : string;
    }





    type UserAssignedToRole = 

        DomainEvent<UserAssignedToRoleEventData>

    and UserAssignedToRoleEventData = {
        GroupId : string
        RoleId : string
        TenantId : string;
    }




    type UserUnAssignedToRole = 

        DomainEvent<UserUnAssignedToRoleEventData>

    and UserUnAssignedToRoleEventData = {
        GroupId : string
        RoleId : string
        TenantId : string;
    }















module Tenant =

    open Events
    open Role
    open User
    










    ///Tenant related types
    /// 
    /// 
    /// 
    /// 
    type ActivationStatus = 
        |Activated 
        |Disactivated


    type RegistrationInvitation = {
        RegistrationInvitationId: RegistrationInvitationId
        Description: RegistrationInvitationDescription
        TenantId: TenantId
        StartingOn: DateTime
        Until: DateTime
        }


    type Tenant = {
        TenantId: TenantId
        Name: TenantName
        Description: TenantDescription 
        RegistrationInvitations: RegistrationInvitation list
        ActivationStatus : ActivationStatus
        }


 



    type TenantActivationStatusActivated = 

        DomainEvent<TenantActivationStatusActivatedEventData>

    and TenantActivationStatusActivatedEventData = {
        TenantId : TenantId
        UserId : UserId
        }


        


    



    type TenantActivationStatusDiactivated = 

        DomainEvent<TenantActivationStatusDiactivatedEventData>

    and TenantActivationStatusDiactivatedEventData = {
        TenantId : TenantId
        UserId : UserId 
        }    



    

    type TenantAdministratorRegistered = 

        DomainEvent<TenantAdministratorRegisteredEventData>

    and TenantAdministratorRegisteredEventData = {
        TenantId : string
        FullName : string
        EmailAddress : string
        TenantName : string
        temporaryPassword : string
        Username : string
        }






    type TenantProvisioned = 
        
        DomainEvent<TenantProvisionedEventData> 
    
    and TenantProvisionedEventData = {
        TenantId : string
        }
      



    type RegistrationInvitationDtoTemp = {
        RegistrationInvitationId : string
        TenantId : string
        Description : string
        StartingOn: DateTime
        Until : DateTime
        }



    type InvitationDescriptor = {
        RegistrationInvitationId : string
        TenantId : string
        RegistrationInvitationDescription : string
        StartingOn : DateTime
        Until : DateTime
        }
    



    type Provision = (Tenant*User*Role)












///Should be a common domain type 
type DateTimeSpan = private {
    Start: DateTime
    End: DateTime
} 












///Miscellaniuous types
/// 
/// 
/// 
/// 
/// 
/// 
/// 
/// 






            
            

            
        

        

