using Framework.Common;
using Framework.Scripts.Common;
namespace Framework.Scripts.Core.Damage
{
    public static class DamageProcessor
    {
        
        public static void ProcessDamage(IDamageProvider Provider, IDamageReceiver Receiver)
        {
            if (Provider.gameObject == Receiver.gameObject)
            {
                return;
            } 
            console.log(typeof(DamageProcessor), "Processing damage: " + Provider.GetDamage() + " from " + Provider.gameObject.name + " to " + Receiver.gameObject.name);
            Receiver.TakeDamage(Provider, Provider.GetDamage()); 
            Provider.OnDamageDealt(Receiver, Provider.GetDamage()); 
        }
    }
}