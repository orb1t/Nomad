﻿using System;
using System.ServiceModel;
using Nomad.Communication.EventAggregation;
using Nomad.Distributed.Communication.Deliveries.SingleDelivery;
using Nomad.Distributed.Communication.Deliveries.TimedDelivery;
using Nomad.Distributed.Communication.Deliveries.TopicDelivery;
using Nomad.Messages.Distributed;

namespace Nomad.Distributed.Communication
{
	[ServiceContract]
	public interface IDistributedEventAggregator
	{
		/// <summary>
		///		Invoked by other  <see cref="DistributedEventAggregator"/> instances 
		/// to send <c>control</c> messages for internal <see cref="Nomad"/> use.
		/// </summary>
		/// <param name="message"></param>
		[OperationContract]
		void OnPublishControl(NomadDistributedMessage message);

		/// <summary>
		///		Invoked by other <see cref="DistributedEventAggregator"/> instances
		/// to send <c>user defined messages</c>.
		/// </summary>
		/// <remarks>
		///		Called by <see cref="ITopicDeliverySubsystem"/>.
		/// </remarks>
		[OperationContract]
		bool OnPublish(byte[] byteStream, TypeDescriptor typeDescriptor);

		/// <summary>
		///		Invoked by other <see cref="DistributedEventAggregator"/> especially <see cref="ISingleDeliverySubsystem"/>
		/// for implementing <see cref="IEventAggregator.PublishSingleDelivery{T}"/>.
		/// </summary>
		/// <param name="byteStream"></param>
		/// <param name="typeDescriptor"></param>
		/// <returns>If deliver was successful (was somebody waiting for this message) then mark it true. Otherwise false.</returns>
		[OperationContract]
		bool OnPublishSingleDelivery(byte[] byteStream, TypeDescriptor typeDescriptor);

		/// <summary>
		///		Invoked by other <see cref="DistributedEventAggregator"/> instances (especially <see cref="ITimedDeliverySubsystem"/>
		/// to send some user defined message with <see cref="IEventAggregator.PublishTimelyBuffered{T}"/>
		/// </summary>		
		[OperationContract]
		void OnPublishTimelyBufferedDelivery(byte[] byteSteam, TypeDescriptor typeDescriptor,DateTime voidTime);
	}
}