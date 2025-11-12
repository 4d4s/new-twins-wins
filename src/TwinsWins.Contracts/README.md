# TwinsWins TON Smart Contracts

This directory contains the TON blockchain smart contracts for the TwinsWins game.

## Overview

The TwinsWins smart contract system uses a factory pattern:
- **Factory Contract**: Deploys individual game instances
- **Game Contract**: Handles escrow, payouts, and game settlement

## Structure

```
TwinsWins.Contracts/
├── contracts/
│   ├── factory.fc          # Factory contract (FunC)
│   ├── game.fc             # Game instance contract (FunC)
│   └── stdlib.fc           # Standard library
├── wrappers/
│   ├── Factory.ts          # TypeScript wrapper
│   └── Game.ts             # TypeScript wrapper
├── tests/
│   ├── Factory.spec.ts     # Factory tests
│   └── Game.spec.ts        # Game tests
├── scripts/
│   ├── deploy-factory.ts   # Deploy factory
│   └── create-game.ts      # Create game instance
└── package.json            # Dependencies
```

## Prerequisites

- Node.js 18+
- TON development tools
- Blueprint (TON development framework)

## Installation

```bash
npm install
```

## Development

### 1. Initialize Blueprint

```bash
npm init ton
```

### 2. Write Contracts

Contracts are written in FunC (TON's smart contract language).

**Factory Contract** (`contracts/factory.fc`):
- Deploys game instances
- Manages game registry
- Collects platform fees

**Game Contract** (`contracts/game.fc`):
- Escrows player stakes
- Validates game results
- Distributes payouts
- Handles refunds

### 3. Compile Contracts

```bash
npm run build
```

### 4. Test Contracts

```bash
npm test
```

### 5. Deploy to Testnet

```bash
npm run deploy:testnet
```

## Contract Features

### Factory Contract

**Methods:**
- `create_game(stake_amount, creator_address)` - Create new game
- `get_game_address(game_id)` - Get game contract address
- `get_total_games()` - Total games created

**Storage:**
- Game counter
- Platform wallet address
- Fee percentages

### Game Contract

**Methods:**
- `join_game(joiner_address)` - Second player joins
- `submit_result(player_address, score)` - Submit player score
- `settle_game()` - Settle game and distribute funds
- `cancel_game()` - Cancel expired game and refund

**Storage:**
- Creator address
- Joiner address
- Stake amount
- Creator score
- Joiner score
- Game status
- Expiration time

**States:**
- 0: Created (waiting for joiner)
- 1: Active (both players joined)
- 2: Settling (results submitted)
- 3: Settled (funds distributed)
- 4: Cancelled (refunded)

## Fee Structure

- **Platform Fee**: 15% of pot
- **Affiliate Fee**: 3% of pot (if applicable)
- **Winner Payout**: 82% of pot (or 85% without affiliate)

## Security Features

1. **Escrow**: Funds locked until game completion
2. **Timeout**: Automatic refund after expiration
3. **Validation**: Server-validated results
4. **Atomic Settlement**: All-or-nothing payouts

## Testing

```bash
# Run all tests
npm test

# Run specific test
npm test Game.spec.ts

# Coverage
npm run test:coverage
```

## Deployment

### Testnet Deployment

```bash
# Set testnet configuration
export TON_NETWORK=testnet

# Deploy factory
npm run deploy:testnet

# Save factory address
export FACTORY_ADDRESS=<your_factory_address>
```

### Mainnet Deployment

⚠️ **Before mainnet deployment:**
1. Complete professional security audit
2. Test extensively on testnet
3. Review all economic parameters
4. Prepare emergency pause mechanism

```bash
# Set mainnet configuration
export TON_NETWORK=mainnet

# Deploy factory (requires confirmation)
npm run deploy:mainnet
```

## Integration with Backend

The backend `BlockchainService.cs` should:

1. **Create Game**:
   ```csharp
   var gameAddress = await FactoryContract.CreateGameAsync(stakeAmount);
   ```

2. **Monitor Game**:
   ```csharp
   var status = await GameContract.GetStatusAsync(gameAddress);
   ```

3. **Settle Game**:
   ```csharp
   await GameContract.SettleGameAsync(winnerAddress, score);
   ```

## TON SDK Integration

Install TON SDK in backend:
```bash
dotnet add package TonSdk.NET
```

## Example Contract Interactions

### Creating a Game

```typescript
import { Factory } from './wrappers/Factory';

const factory = Factory.createFromAddress(FACTORY_ADDRESS);
const gameAddress = await factory.createGame(
  toNano('1'), // 1 TON stake
  creatorAddress
);
```

### Joining a Game

```typescript
import { Game } from './wrappers/Game';

const game = Game.createFromAddress(gameAddress);
await game.joinGame(joinerAddress);
```

### Settling a Game

```typescript
await game.submitResult(player1Address, score1);
await game.submitResult(player2Address, score2);
await game.settleGame();
```

## Resources

- [TON Documentation](https://docs.ton.org/)
- [FunC Documentation](https://docs.ton.org/develop/func/overview)
- [Blueprint Framework](https://github.com/ton-community/blueprint)
- [TON SDK](https://github.com/ton-community/ton)

## TODO

- [ ] Implement factory.fc contract
- [ ] Implement game.fc contract
- [ ] Write comprehensive tests
- [ ] Add TypeScript wrappers
- [ ] Deploy to testnet
- [ ] Professional security audit
- [ ] Deploy to mainnet

## Support

For TON development questions:
- [TON Dev Chat](https://t.me/tondev)
- [TON Overflow](https://answers.ton.org/)

---

**Status**: Structure defined, implementation needed
**Priority**: High (required for MVP)
