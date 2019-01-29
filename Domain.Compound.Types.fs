namespace IdentityAndAcccess.DomainTypes


///Imported local libs
open IdentityAndAcccess.CommonDomainTypes
open System
open System.Text.RegularExpressions













[<AutoOpen>]
module Events =


    


    ///Useful types
    /// 
    /// 
    /// 
    /// 
    /// 
    type Event<'Data, 'Type> = {
        EventVersion : int
        OccurredOn : DateTime
        Data : 'Data
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
        | Enabled 
        | Disabled


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

        Event<ContactInformationChangedData>

    and ContactInformationChangedData = {
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

        Event<PersonalNameChangedData>

    and PersonalNameChangedData = {
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

    type UserRegisteredChangedEventData = {
        UserId: string
        TenantId: string
        Username: string
        Password: string
        Enablement: string
        Person: string
        BirthDate : string
        }


    type RoleDescriptor = {
            RoleId: RoleId
            TenantId: TenantId
            Name: RoleName
            }



    type UserDescriptor = {
        UserDescriptorId: UserDescriptorId
        TenantId: TenantId
        Username: Username
        Email: EmailAddress
        Roles: RoleDescriptor list 
        }


    

    type UserEnablementChanged =

            Event<UserEnablementChangedEventData>

    and UserEnablementChangedEventData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        }




    



    type UserPasswordChanged = 

            Event<UserPasswordChangedEventData>

    and UserPasswordChangedEventData = {
        EventVersion: int
        OccurredOn : DateTime
        TenantId: string
        Username: string
        }


    
    
 


    type UserRegistered = 

        Event<UserRegisteredData>

    and UserRegisteredData = {
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
        MemberIn : GroupMember list
        }



    type Group = 
        | Standard of StandardGroup
        | Internal of StandardGroup



     
    type GroupProvisioned = 

        Event<GroupProvisionedEventData>

    and GroupProvisionedEventData = {
        GroupId : string
        TenantId :string
        GroupName :string 
        }

    type MemberAddedToGroupEvent = { 
        GroupId : string
        TenantId : string
        MemberAdded : GroupMember
        }

    type MemberInAddedToGroupEvent = { 
        GroupId : string
        TenantId : string
        MemberInAdded : GroupMember
        }

    

    type MemberAddedToGroup = 
        Event<MemberAddedToGroupData>
    and MemberAddedToGroupData = {
        TenantId : string
        MemberAdded : GroupMember
        }

    type MemberInAddedToGroup = 

        Event<MemberInAddedToGroupData>

    and MemberInAddedToGroupData = {
        TenantId : string
        MemberInAdded : GroupMember
        }  



    type GroupGroupRemoved = 

        Event<GroupGroupRemovedData>
    
    and GroupGroupRemovedData = {

        GroupId: string
        RemovedGroupId: string
        TenantId: string

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

        Event<RoleProvisionedData>

    and RoleProvisionedData = {
        RoleId : string
        TenantId : string;
    }



    type GroupAssignedToRole = 

        Event<GroupAssignedToRoleEventData>

    and GroupAssignedToRoleEventData = {
        GroupId : string
        RoleId : string
        TenantId : string;
    }




    type GroupUnAssignedToRole = 

        Event<GroupUnAssignedToRoleEventData>

    and GroupUnAssignedToRoleEventData = {
        GroupId : string
        RoleId : string
        TenantId : string;
    }





    type UserAssignedToRole = 

        Event<UserAssignedToRoleEventData>

    and UserAssignedToRoleEventData = {
        GroupId : string
        RoleId : string
        TenantId : string;
    }




    type UserUnAssignedToRole = 

        Event<UserUnAssignedToRoleEventData>

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
        | Activated 
        | Deactivated


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


    type TenantProvisioned = 
        Event<TenantProvisionedData>
    and TenantProvisionedData = {
        TenantId : string; 
        GroupId : string
        RoleId : string
    }



    type TenantActivationStatusActivated = 

        Event<TenantActivationStatusActivatedEventData>

    and TenantActivationStatusActivatedEventData = {
        TenantId : TenantId
        UserId : UserId
        }


        


    



    type TenantActivationStatusDiactivated = 

        Event<TenantActivationStatusDiactivatedEventData>

    and TenantActivationStatusDiactivatedEventData = {
        TenantId : TenantId
        UserId : UserId 
        }    



    

    type TenantAdministratorRegistered = 

        Event<TenantAdministratorRegisteredEventData>

    and TenantAdministratorRegisteredEventData = {
        TenantId : string
        FullName : string
        EmailAddress : string
        TenantName : string
        temporaryPassword : string
        Username : string
        }










    type InvitationDescriptor = {
        RegistrationInvitationId : string
        TenantId : string
        RegistrationInvitationDescription : string
        StartingOn : DateTime
        Until : DateTime
        }
    


    type TenantProvision = {
        Tenant : Tenant
        AdminUser : User
        AdminRole : Role 
        OfferredInvitation : RegistrationInvitation  
        WithdrawnInvitation : RegistrationInvitation
        }


    //type Provision = (Tenant*User*Role*RegistrationInvitation list)












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






            
            

            
        

        

