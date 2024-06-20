import subprocess
import sys
import asyncio
# Ensure bleak is installed
try:
    import bleak
except ImportError:
    subprocess.check_call([sys.executable, "-m", "pip", "install", "bleak"])
    import bleak

from bleak import BleakClient

address = "12:E9:00:28:AD:83"  # Replace with your BLE device address
characteristic_uuid = "0000ffe2-0000-1000-8000-00805f9b34fb"  # Replace with the correct characteristic UUID

async def send_data(data):
    async with BleakClient(address) as client:
        await client.write_gatt_char(characteristic_uuid, data.encode('utf-8'))
        print(f"Sent data: {data}")

if __name__ == "__main__":
# data = sys.argv[1]
    data = "255255255255255255255255255255"
    asyncio.run(send_data("<" + data + ">"))
