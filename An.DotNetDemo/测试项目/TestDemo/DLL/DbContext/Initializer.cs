using DLL.Eum;
using DLL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL
{
    public class Initializer :DropCreateDatabaseIfModelChanges<MyDbContext>
    {
        protected override void Seed(MyDbContext context)
        {
            context.Management.AddRange(new List<ManagementModel>
            {
                new ManagementModel(){  LoginId="Admin" , LoginPwd="admin123", IsState=true, IsDelete=false  },
                 new ManagementModel(){  LoginId="Admin1" , LoginPwd="admin1234", IsState=true, IsDelete=true  }
            });
            context.User.AddRange(new List<UserModel>
            {
                 new UserModel(){
                      RealName="Jack",
                     Age =20,
                     Gender =(int)EumGanderType.Male,
                      Email="jack@qq.com",
                       Addres="上海",
                        Mobile="1300000000",
                         IsDelete=false,
                          IsState=true

                 },
                   new UserModel(){
                      RealName="Lisa",
                     Age =20,
                     Gender =(int)EumGanderType.Female,
                      Email="Lisa@qq.com",
                       Addres="北京",
                        Mobile="1300000000",
                         IsDelete=false,
                          IsState=false

                 }
            });
            context.SaveChanges();
        }
    }
}
