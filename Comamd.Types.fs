namespace IdentityAndAcccess.CommandTypes




open System








module Commands =







    type Command<'data> = {
        Data : 'data;
        TimeStamp : DateTime;
        UserId : string;
    }




    type ProvisionTenantCommand =
            Command<ProvisionTenantData> 
    and ProvisionTenantData = {

         TenantName : string

    }



    type InviteUser =
            Command<InviteUserData> 
    and InviteUserData = {
         RegistrationInvitationId : string
    }



    type CreateRole =
                Command<CreateRoleData> 
    and CreateRoleData = {
         RoleName : string
    }



    type CreateGroup =
                Command<CreateGroupData> 
    and CreateGroupData = {
         GroupName : string
    }




    type RegisterUser =
                Command<RegisterUserData> 
    and RegisterUserData = {
         Username : string
    }


    

    type ChangePassword =
                Command<ChangePasswordData> 
    and ChangePasswordData = {
         Password : string
    }



    type ChangeContactInformation =
                Command<ChangeContactInformationData> 
    and ChangeContactInformationData = {
         Email : string
         Phone : string
    }




    type ChangePersonalName =
                Command<ChangePersonalNameData> 
    and ChangePersonalNameData = {
         Username : string
    }



    type ActivateTenantActivationStatus =
                Command<ActivateTenantActivationStatusData> 
    and ActivateTenantActivationStatusData = {
         Username : string
    }



    type DeactivateTenantActivationStatus =
                Command<DeactivateTenantActivationStatusData> 
    and DeactivateTenantActivationStatusData = {
         Username : string
    }



    type EnableUSerEnablementStatus =
                Command<EnableUSerEnablementStatusData> 
    and EnableUSerEnablementStatusData = {
         Username : string
    }



    type DesableUSerEnablementStatus =
                Command<DesableUSerEnablementStatusData> 
    and DesableUSerEnablementStatusData = {
         Username : string
    }



    type AddUserToGroup =
                Command<AddUserToGroupData> 
    and AddUserToGroupData = {
         Username : string
    }



    type AddGroupToGroup =
                Command<AddGroupToGroupData> 
    and AddGroupToGroupData = {
         Username : string
    }




    type RemoveUserFromGroup =
                Command<RemoveUserFromGroupData> 
    and RemoveUserFromGroupData = {
         Username : string
    }


    type RemoveGroupFromGroup =
                Command<RemoveGroupFromGroupData> 
    and RemoveGroupFromGroupData = {
         Username : string
    }


    type AddUserToRole =
                Command<AddUserToRoleData> 
    and AddUserToRoleData = {
         Username : string
    }


    type AddGroupToRole =
                Command<AddGroupToRoleData> 
    and AddGroupToRoleData = {
         Username : string
    }


    type RemoveUserFromRole =
                Command<RemoveUserFromRoleData> 
    and RemoveUserFromRoleData = {
         Username : string
    }


    type RemoveGroupFromRole =
                Command<RemoveGroupFromRoleData> 
    and RemoveGroupFromRoleData = {
         Username : string
    }










    type IdentityAndAcccessCommand = 
        | CreateRole of CreateRole
        | CreateGroup of CreateGroup
        | InviteUser of InviteUser  
        | RegisterUser of RegisterUser
        | ChangePassword of ChangePassword
        | ChangeContactInformation of ChangeContactInformation
        | ChangePersonalName of ChangePersonalName
        | ActivateTenantActivationStatus of ActivateTenantActivationStatus
        | DeactivateTenantActivationStatus of DeactivateTenantActivationStatus
        | EnableUSerEnablementStatus of EnableUSerEnablementStatus
        | DesableUSerEnablementStatus of DesableUSerEnablementStatus
        | AddUserToGroup of AddUserToGroup
        | AddGroupToGroup of AddGroupToGroup
        | RemoveUserFromGroup of RemoveUserFromGroup
        | RemoveGroupFromGroup of RemoveGroupFromGroup
        | AddUserToRole of AddUserToRole
        | AddGroupToRole of AddGroupToRole
        | RemoveUserFromRole of RemoveUserFromRole
        | RemoveGroupFromRole of RemoveGroupFromRole
