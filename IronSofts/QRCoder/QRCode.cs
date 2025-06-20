namespace QRCoder
{
    internal class QRCode : IDisposable
    {
        private QRCodeData qrData;

        public QRCode(QRCodeData qrData)
        {
            this.qrData = qrData;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        internal IDisposable GetGraphic(int v)
        {
            throw new NotImplementedException();
        }
    }
}