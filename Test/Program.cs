using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qwerty.DAL.Repositories;
using Qwerty.DAL.Entities;
using System.Configuration;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            #region com
            //string connection = ConfigurationManager.ConnectionStrings["Model1"].ConnectionString;
            //UnitOfWork unitOfWork = new UnitOfWork(connection);
            //Friend user2 = unitOfWork.FriendManager.GetAll().Last();
            //foreach (var Users in user2.Users)
            //{
            //    Console.WriteLine(Users.UserProfile.Name);
            //}
            //unitOfWork.FriendManager.Delete(user2.FriendId);
            //Console.ReadLine();
            #endregion
            TestDB();
            Console.ReadLine();
        }
        public static void TestDB()
        {
            string connection = ConfigurationManager.ConnectionStrings["Model1"].ConnectionString;
            using (UnitOfWork unitOfWork = new UnitOfWork(connection))
            {
                User user2 = unitOfWork.QUserManager.GetAll().Last();
                User user1 = unitOfWork.QUserManager.GetAll().First();
                #region cooments
                /*
                //FriendshipRequest friendshipRequest = new FriendshipRequest()
                //{
                //    RecipientUser = user1,
                //    RecipientUserId = user1.UserId,

                //    SenderUser = user2,
                //    SenderUserId = user2.UserId,

                //    Status = FriendshipRequestStatus.NotSent,
                //    TimeSent = DateTime.Now
                //};
                //unitOfWork.RequestManager.Create(friendshipRequest);
                ////Friend friend = new Friend()
                ////{
                ////    FriendId = user1.UserId,
                ////    UserProfile = user1.UserProfile,
                ////};
                ////friend.Users.Add(user2);
                //// user2.Friends.Add(friend);
                //await unitOfWork.SaveAsync();
                */
                #endregion
                foreach (var request in user1.ReciveFriendshipRequests)
                {
                    if (request.SenderUser.UserProfile.Name == "Admin")
                    {
                        request.Status = FriendshipRequestStatus.Accepted;
                        if (request.Status == FriendshipRequestStatus.Accepted)
                        {
                            Friend friend = new Friend()
                            {
                                FriendId = request.SenderUserId,
                                UserProfile = request.SenderUser.UserProfile
                            };
                            friend.Users.Add(user2);
                            unitOfWork.FriendManager.Create(friend);
                        }
                        if (request.Status == FriendshipRequestStatus.Accepted || request.Status == FriendshipRequestStatus.Rejected)
                        {
                            unitOfWork.RequestManager.Delete(request.RecipientUserId, request.SenderUserId);
                        }
                        break;
                    }
                }
            }
            Console.ReadLine();
        }
    }
}
