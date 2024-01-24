using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
using TopLearn.Core.Genrator;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.Core.Services
{
    public class UserService : IUserInterface
    {
        TopLearnContext _context;
        public UserService(TopLearnContext context)
        {
            _context = context;
        }

        public bool ActiveAccount(string ActiveCode)
        {
            var user = _context.Users.SingleOrDefault(x => x.ActiveCode == ActiveCode);
            if (user == null || user.IsActive)
            {
                return false;
            }
            user.IsActive = true;
            user.ActiveCode = GetUserActiveCode.GetActiveCode();
            _context.SaveChanges();

            return true;

        }

        public void AddMoney(string UserName, int Amount, string Description, bool IsPay = false)
        {
            int UserId = GetUserIdByUserName(UserName);

            Wallet wallet = new Wallet()
            {
                Amount = Amount,
                CreateDate = DateTime.Now,
                IsPay = IsPay,
                Description = Description,
                TypeId = 2,
                UserId = UserId,
            };
            AddWallet(wallet);
        }

        public int AddUser(User user)
        {
            _context.Add(user);
            _context.SaveChanges();
            return user.UserId;
        }

        public void AddWallet(Wallet wallet)
        {
            _context.Wallet.Add(wallet);
            _context.SaveChanges();
        }

        public int BalanceUserWallet(string username)
        {
            int UserId = GetUserIdByUserName(username);

            var deposit = _context.Wallet.Where(x => x.UserId == UserId && x.TypeId == 2 && x.IsPay == true).Select(x => x.Amount);
            var Withdraw = _context.Wallet.Where(x => x.UserId == UserId && x.TypeId == 1 && x.IsPay == true).Select(x => x.Amount);
            return (deposit.Sum()) - (Withdraw.Sum());

        }

        public bool CheckTheOldPass(string username, string oldpassword)
        {
            var hashPass = PasswordHelper.EncodePasswordMd5(oldpassword);
            return _context.Users.Any(x => x.UserName == username && x.Password == hashPass);
        }

        public void EditPassWord(string username, string pass)
        {
            var user = GetByUserName(username);
            user.UserName = username;
            user.Password = PasswordHelper.EncodePasswordMd5(pass);
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void EditProfile(string username, EditProfileViewModel editProfile)
        {
            if (editProfile.Avatar != null)
            {
                string Imagepath = "";
                if (editProfile.AvatarName != "Defult.jpg")
                {
                    Imagepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", editProfile.AvatarName);
                    if (File.Exists(Imagepath))
                    {
                        File.Delete(Imagepath);
                    }
                }
                editProfile.AvatarName = GetUserActiveCode.GetActiveCode() + Path.GetExtension(editProfile.Avatar.FileName);
                Imagepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", editProfile.AvatarName);
                using (var stream = new FileStream(Imagepath, FileMode.Create))
                {
                    editProfile.Avatar.CopyTo(stream);
                }
            }
            var user = GetByUserName(username);
            user.UserName = editProfile.UserName;
            user.Email = editProfile.Email;
            user.UserAvatar = editProfile.AvatarName;

            _context.Update(user);
            _context.SaveChanges();
        }

        public User GetByActiveCode(string ActiveCode)
        {
            return _context.Users.SingleOrDefault(x => x.ActiveCode == ActiveCode);
        }

        public User GetByEmail(string email)
        {
            return _context.Users.SingleOrDefault(x => x.Email == email);
        }

        public User GetByUserName(string UserName)
        {
            return _context.Users.SingleOrDefault(x => x.UserName == UserName);
        }

        public EditProfileViewModel GetEditProfile(string UserName)
        {
            return _context.Users.Where(y => y.UserName == UserName).Select(x => new EditProfileViewModel()
            {
                UserName = x.UserName,
                AvatarName = x.UserAvatar,
                Email = x.Email,
            }).SingleOrDefault();
        }

        public SideBarUserPanelViewModel GetSidebarUserPanel(string UserName)
        {
            return _context.Users.Where(y => y.UserName == UserName).Select(x => new SideBarUserPanelViewModel()
            {
                UserName = x.UserName,
                ImageName = x.UserAvatar,
                RegisterTime = x.RegisterDate,
            }).SingleOrDefault();
        }

        public int GetUserIdByUserName(string username)
        {
            return _context.Users.SingleOrDefault(x => x.UserName == username).UserId;
        }

        public InformationUserViewModel GetUserInformation(string UserName)
        {
            var user = GetByUserName(UserName);
            if (user == null)
            {
                return null;
            }
            InformationUserViewModel informationUserViewModel = new InformationUserViewModel()
            {
                Email = user.Email,
                UserId = user.UserId,
                RegisterDate = user.RegisterDate,
                UserName = user.UserName,
                Wallet = BalanceUserWallet(UserName)
            };
            return informationUserViewModel;

        }

        public List<WalletViewModel> GetUserWallet(string username)
        {
            int UserId = GetUserIdByUserName(username);
            return _context.Wallet.Where(x => x.IsPay == true && UserId == UserId).Select(x => new WalletViewModel()
            {
                Amount = x.Amount,
                DateTime = x.CreateDate,
                Description = x.Description,
                Type = x.TypeId
            }).ToList();
        }

        public bool IsExistUserName(string userName)
        {
            return _context.Users.Any(u => u.UserName == userName);
        }

        public int MaxId()
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(User user)
        {
            _context.Update(user);
            _context.SaveChanges();
        }

        bool IUserInterface.IsEmailExist(string Email)
        {
            return _context.Users.Any(x => x.Email == Email);
        }

        User IUserInterface.LoginUser(LoginViewModel user)
        {
            string hashPassword = PasswordHelper.EncodePasswordMd5(user.Password);
            string email = TextFixer.TextFixed(user.Email);
            return _context.Users.SingleOrDefault(y => y.Email == email && y.Password == hashPassword);
        }
    }
}
