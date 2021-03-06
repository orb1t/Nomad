using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using Nomad.Core;
using Version = Nomad.Utils.Version;

namespace Nomad.Distributed.Communication.Utils
{
	/// <summary>
	///		Helper class for performing serialization and deserialization
	/// of messages sent by <see cref="DistributedEventAggregator"/>.
	/// </summary>
	public static class MessageSerializer
	{
		private static readonly ILog Logger = LogManager.GetLogger(NomadConstants.NOMAD_LOGGER_REPOSITORY,
		                                                           typeof (MessageSerializer));
		
		// TODO: do something about this kind of constant
		private static decimal MESSAGE_SIZE = 65000;

		public static object Deserialize(byte[] bytes)
		{
			using (var stream = new MemoryStream(bytes))
			{
				try
				{
					IFormatter formatter = new BinaryFormatter();
					return formatter.Deserialize(stream);
				}
				catch (Exception e)
				{
					Logger.Warn("DeSerialization warning: ", e);
					throw;
				}
			}
		}

		public static byte[] Serialize(Object obj)
		{
			byte[] bytes;
			try
			{
				IFormatter formatter = new BinaryFormatter();
                using (var stream = new MemoryStream())
                {
                    formatter.Serialize(stream, obj);

                    if (stream.Length > MESSAGE_SIZE)
                    {
                        throw new InvalidOperationException("Object is to large for serialization");
                    }
                    
                    bytes = stream.ToArray();
                }
			}
			catch (Exception e)
			{
				Logger.Warn("Serialization warning: ", e);
				throw;
			}

			return bytes;
		}

		internal static void UnPackData(TypeDescriptor typeDescriptor, byte[] byteStream, out object sendObject, out Type type)
		{
			type = Type.GetType(typeDescriptor.QualifiedName);
			if (type != null)
			{
				var nomadVersion = new Version(type.Assembly.GetName().Version);

				if (!nomadVersion.Equals(typeDescriptor.Version))
				{
					throw new InvalidCastException("The version of the assembly does not match");
				}
			}

			// try deserializing object
			sendObject = Deserialize(byteStream);

			// check if o is assignable
			if (type != null && !type.IsInstanceOfType(sendObject))
			{
				throw new InvalidCastException("The sent object cannot be casted to sent type");
			}
		}
	}
}