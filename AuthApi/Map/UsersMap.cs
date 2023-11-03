using AuthApi.Map;
using AuthApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace AuthApi.Map
{
    public class UsersMap : BaseMap<User>
    {
        public UsersMap() : base("tb_Users") { }

        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.UserName).HasColumnName("username").HasColumnType("varchar(100)").IsRequired();
            builder.Property(x => x.Role).HasColumnName("role").HasColumnType("varchar(100)").IsRequired();
            builder.Property(x => x.BirthDay).HasColumnName("birthday").HasColumnType("varchar(100)").IsRequired();
            builder.Property(x => x.PasswordSalt).HasColumnName("passwordsalt").HasColumnType("varchar(256)").IsRequired();
            builder.Property(x => x.PasswordHash).HasColumnName("passwordhash").HasColumnType("varchar(256)").IsRequired();
            builder.Property(x => x.Token).HasColumnName("token").HasColumnType("varchar(256)").IsRequired(false);
        }

    }
}
