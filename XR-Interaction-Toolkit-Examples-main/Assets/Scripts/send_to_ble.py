import sys
import asyncio
from bleak import BleakClient

address = "12:E9:00:28:AD:83"  # Replace with your BLE device address
characteristic_uuid = "0000ffe2-0000-1000-8000-00805f9b34fb"  # Replace with the correct characteristic UUID

async def send_data(data):
    async with BleakClient(address) as client:
        await client.write_gatt_char(characteristic_uuid, data.encode('utf-8'))
        print(f"Sent data: {data}")

if __name__ == "__main__":
    data = sys.argv[1]
    asyncio.run(send_data("<"+data+">"))