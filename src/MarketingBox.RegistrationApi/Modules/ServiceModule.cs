using Autofac;
using MarketingBox.Affiliate.Service.MyNoSql.Boxes;
using MarketingBox.Registration.Service.Client;
using MyJetWallet.Sdk.NoSql;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;

namespace MarketingBox.RegistrationApi.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterRegistrationServiceClient(Program.Settings.RegistrationServiceUrl);
            var noSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));

            var subs = new MyNoSqlReadRepository<BoxNoSql>(noSqlClient, BoxNoSql.TableName);
            builder.RegisterInstance(subs)
                .As<IMyNoSqlServerDataReader<BoxNoSql>>();
        }
    }
}
