using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Bank2Qif;
using Bank2Qif.Transformers;
using Nini.Config;
using Moq;
using Bank2Qif.Transformers.Default;

namespace Bank2QifTests.TransformersTests
{    
    [TestFixture]
    public class DefaultTransformerTest
    {        
        private QifEntry entry;
        private IConfig m_config;


        [SetUp]
        public void SetUp()
        {
            entry = new QifEntry 
            {
                Description = "Name",            
            };
            var mockConfig = new Mock<IConfig>();
            mockConfig.Setup(c => c.GetString("Account", "QifImport")).Returns("QifImport");
            m_config = mockConfig.Object;
        }


        [Test]
        public void TestIsDefaultAssigned()
        {
            var transformer = new DefaultTransformer(m_config);
            var res = transformer.Transform(new List<QifEntry> { entry });

            Assert.AreEqual("QifImport", res.Single().AccountName);
        }
    }
}
