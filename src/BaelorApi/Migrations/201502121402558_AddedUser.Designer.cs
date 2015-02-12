using BaelorApi.Models.Database;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Infrastructure;
using System;

namespace BaelorApi.Migrations
{
    [ContextType(typeof(DatabaseContext))]
    public partial class AddedUser : IMigrationMetadata
    {
        string IMigrationMetadata.MigrationId
        {
            get
            {
                return "201502121402558_AddedUser";
            }
        }
        
        string IMigrationMetadata.ProductVersion
        {
            get
            {
                return "7.0.0-beta1-11518";
            }
        }
        
        IModel IMigrationMetadata.TargetModel
        {
            get
            {
                var builder = new BasicModelBuilder();
                
                builder.Entity("BaelorApi.Models.Database.Album", b =>
                    {
                        b.Property<DateTime>("CreatedAt");
                        b.Property<string>("Genres");
                        b.Property<Guid>("Id")
                            .GenerateValuesOnAdd();
                        b.Property<Guid>("ImageId");
                        b.Property<string>("Label");
                        b.Property<int>("LengthSeconds");
                        b.Property<string>("Name");
                        b.Property<string>("Producers");
                        b.Property<DateTime>("ReleasedAt");
                        b.Property<string>("Slug");
                        b.Property<DateTime>("UpdatedAt");
                        b.Key("Id");
                    });
                
                builder.Entity("BaelorApi.Models.Database.Image", b =>
                    {
                        b.Property<DateTime>("CreatedAt");
                        b.Property<string>("FilePath");
                        b.Property<Guid>("Id")
                            .GenerateValuesOnAdd();
                        b.Property<DateTime>("UpdatedAt");
                        b.Key("Id");
                    });
                
                builder.Entity("BaelorApi.Models.Database.Song", b =>
                    {
                        b.Property<Guid>("AlbumId");
                        b.Property<DateTime>("CreatedAt");
                        b.Property<Guid>("Id")
                            .GenerateValuesOnAdd();
                        b.Property<int>("Index");
                        b.Property<int>("LengthSeconds");
                        b.Property<string>("Producers");
                        b.Property<string>("Slug");
                        b.Property<string>("Title");
                        b.Property<DateTime>("UpdatedAt");
                        b.Property<string>("Writers");
                        b.Key("Id");
                    });
                
                builder.Entity("BaelorApi.Models.Database.User", b =>
                    {
                        b.Property<string>("ApiKey");
                        b.Property<DateTime>("CreatedAt");
                        b.Property<string>("Email");
                        b.Property<Guid>("Id")
                            .GenerateValuesOnAdd();
                        b.Property<string>("PasswordHash");
                        b.Property<int>("PasswordIterations");
                        b.Property<string>("PasswordSalt");
                        b.Property<DateTime>("UpdatedAt");
                        b.Property<string>("Username");
                        b.Key("Id");
                    });
                
                builder.Entity("BaelorApi.Models.Database.Album", b =>
                    {
                        b.ForeignKey("BaelorApi.Models.Database.Image", "ImageId");
                    });
                
                builder.Entity("BaelorApi.Models.Database.Song", b =>
                    {
                        b.ForeignKey("BaelorApi.Models.Database.Album", "AlbumId");
                    });
                
                return builder.Model;
            }
        }
    }
}