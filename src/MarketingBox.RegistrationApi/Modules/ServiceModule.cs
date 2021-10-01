using Autofac;
using MarketingBox.Affiliate.Service.MyNoSql.Partners;
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
            var partners = new MyNoSqlReadRepository<PartnerNoSql>(noSqlClient, PartnerNoSql.TableName);
            builder.RegisterInstance(partners)
                .As<IMyNoSqlServerDataReader<PartnerNoSql>>();
        }
    }
}
