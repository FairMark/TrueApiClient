﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FairMark.OmsApi;
using FairMark.TrueApi.DataContracts;
using NUnit.Framework;

namespace FairMark.TrueApi.Tests
{
    [TestFixture]
    public class AuthentificationTests : UnitTestsBase
    {
        [Test]
        public void TrueApiClientAuthenticates()
        {
            var client = new TrueApiClient(TrueApiClient.SandboxApiUrl, new TrueApiCredentials
            {
                CertificateThumbprint = TestCertificateThumbprint,
            });

            // test tracing
            var trace = new StringBuilder();
            client.Tracer = (f, a) =>
            {
                trace.AppendFormat(f, a);
                TestContext.Progress.WriteLine(f, a);
            };

            try
            {
                // authenticates and requests a resource
                var s = client.Get("auth/key");
                Assert.NotNull(s);
                Assert.IsTrue(client.Authenticator.IsAuthenticated);
                Assert.NotNull(client.Authenticator.AuthToken);
            }
            finally
            {
                // logs out
                Assert.DoesNotThrow(() => client.Dispose());
                Assert.IsFalse(client.Authenticator.IsAuthenticated);
                Assert.IsNull(client.Authenticator.AuthToken);
            }

            var traceText = trace.ToString();
            Assert.IsTrue(traceText.Length > 0, "TrueApiClient trace is empty");
            Assert.IsTrue(traceText.Contains("// Authenticate"));
            Assert.IsTrue(traceText.Contains("// GetToken"));
            Assert.IsTrue(traceText.Contains("-> GET"));
            Assert.IsTrue(traceText.Contains("<- OK 200 (OK)"));
        }

        [Test]
        public void OmsApiClientAuthenticates()
        {
            var client = new OmsApiClient(OmsApiClient.SandboxApiUrl, OmsApiClient.SandboxAuthUrl, "milk", new OmsCredentials
            {
                CertificateThumbprint = TestCertificateThumbprint,
                OmsID = TestOmsID,
                OmsConnectionID = TestOmsConnectionID,
            });

            // test tracing
            var trace = new StringBuilder();
            client.Tracer = (f, a) =>
            {
                trace.AppendFormat(f, a);
                TestContext.Progress.WriteLine(f, a);
            };

            try
            {
                // authenticates and requests a resource
                //var s = client.Get($"milk/ping?omsId={TestOmsID}");
                var version = client.GetVersion();
                Assert.NotNull(version);
                Assert.IsTrue(client.Authenticator.IsAuthenticated);
                Assert.NotNull(client.Authenticator.AuthToken);
            }
            finally
            {
                // logs out
                Assert.DoesNotThrow(() => client.Dispose());
                Assert.IsFalse(client.Authenticator.IsAuthenticated);
                Assert.IsNull(client.Authenticator.AuthToken);
            }

            var traceText = trace.ToString();
            Assert.IsTrue(traceText.Length > 0, "OmsApiClient trace is empty");
            Assert.IsTrue(traceText.Contains("// Authenticate"));
            Assert.IsTrue(traceText.Contains("// GetToken"));
            Assert.IsTrue(traceText.Contains("-> GET"));
            Assert.IsTrue(traceText.Contains("<- OK 200 (OK)"));
        }
    }
}
