import asyncio
from bleak import BleakScanner


async def scan():
    devices = await BleakScanner.discover()
    for device in devices:
        print(device)

if __name__ == "__main__":
    asyncio.run(scan())