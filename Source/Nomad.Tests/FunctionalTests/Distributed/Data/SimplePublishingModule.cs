using Nomad.Communication.EventAggregation;
using Nomad.Messages.Loading;
using Nomad.Modules;

namespace Nomad.Tests.FunctionalTests.Distributed.Data
{
	/// <summary>
	///		Sample class used for publishing data
	/// </summary>
	public class SimplePublishingModule : IModuleBootstraper
	{
		private IEventAggregator _aggregator;
		private IEventAggregatorTicket<NomadAllModulesLoadedMessage> _allModulesLoadedSubscriptionTicket;

		public SimplePublishingModule(IEventAggregator eventAggregator)
		{
			_aggregator = eventAggregator;
		}

		public void OnLoad()
		{
			_allModulesLoadedSubscriptionTicket = _aggregator.Subscribe<NomadAllModulesLoadedMessage>(StartPublishing);
		}

		private void StartPublishing(NomadAllModulesLoadedMessage obj)
		{
			for (int i = 0; i < 5; i++)
			{
				var payload = "Sample Message " + i;
				var message = new DistributableMessage(payload);
				_aggregator.Publish(message);
			}
		}

		public void OnUnLoad()
		{
			// do nothing
		}
	}
}