using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.DTOs.UserViewModel;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IUserInterface
    {
        public bool IsEmailExist(string Email);
        public bool IsExistUserName(string userName);

        public int AddUser(User user);
        public User LoginUser(LoginViewModel user);
        public User GetByEmail(string email);
        public User GetByUserName(string UserName);
        public User GetByActiveCode(string ActiveCode);
        public int UpdateUser(User user);

        public bool ActiveAccount(string ActiveCode);

        #region UserPanel
        InformationUserViewModel GetUserInformation(string UserName);
        InformationUserViewModel GetUserInformation(int UserId);

        SideBarUserPanelViewModel GetSidebarUserPanel(string UserName);
        EditProfileViewModel GetEditProfile(string UserName);
        void EditProfile(string username, EditProfileViewModel editProfile);
        bool CheckTheOldPass(string username, string oldpass);
        void EditPassWord(string username, string pass);
        int GetUserIdByUserName(string username);


        #endregion

        #region Wallet

        int BalanceUserWallet(string username);
        List<WalletViewModel> GetUserWallet(string username);

        void AddMoney(string UserName, int Amount, string Description, bool IsPay = false);
        void AddWallet(Wallet wallet);
        int MaxId();
        #endregion
        #region Admin Panel
        UserForAdminViewModel GetUsers(int PageId=1,string FilterEmail="",string FilterUserName="");
        int AddUserFromAdmin(CreateUserViewModel user);
        int UpdateUserFromAdmin(EditUserViewModel user);
        public EditUserViewModel GetByUserIdForEditAdmin(int UserId);
        public User GetByUserByUserIdEditAdmin(int UserId);

        public UserForAdminViewModel GetDeletedUsers(int PageId = 1, string FilterEmail = "", string FilterUserName = "");

        public void DeletingUser(int UserId);

        #endregion
    }
}
