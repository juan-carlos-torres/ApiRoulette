using Roulette.BI.DTORequest.Roulette;
using Roulette.BI.DTOResponse.Roulette;
using Roulette.DAL.DataAccess;
using Roulette.DAL.DataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roulette.BI.Services
{
    public class RouletteService
    {
        private readonly DBConnection _DBConnection;

        public RouletteService(DBConnection DBConnection)
        {
            _DBConnection = DBConnection;
        }

        public async Task<AddRouletteResponseDTO> AddRoulette()
        {
            try
            {
                RouletteDTO informationRoulette = MapRouletteInformation();
                await _DBConnection.AddKey(key: informationRoulette.ID.ToString(), objectToSave: informationRoulette);
                var response = new AddRouletteResponseDTO
                {
                    RouletteId = informationRoulette.ID
                };

                return response;
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }

        private RouletteDTO MapRouletteInformation()
        {
            try
            {
                var newRoulette = new RouletteDTO
                {
                    ID = Guid.NewGuid(),
                    IsOpen = false,
                    BetList = new List<BetDTO>(),
                    WinningNumber = null
                };

                return newRoulette;
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }

        public async Task<OpenRouletteResponseDTO> OpenRoulette(Guid rouletteID)
        {
            try
            {
                RouletteDTO roulette = await _DBConnection.GetByKey<RouletteDTO>(key: rouletteID.ToString());
                roulette.IsOpen = true;
                await _DBConnection.UpdateKey(key: rouletteID.ToString(), objectToUpdate: roulette);
                var response = new OpenRouletteResponseDTO
                {
                    Success = true
                };

                return response;
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }

        public async Task<AddRouletteBetResponseDTO> AddRouletteBet(AddRouletteBetRequestDTO addRouletteBetRequestDTO, Guid userId)
        {
            try
            {
                RouletteDTO roulette = await _DBConnection.GetByKey<RouletteDTO>(key: addRouletteBetRequestDTO.RouletteID.ToString());
                BetDTO newBet = MapBetInformation(addRouletteBetRequestDTO: addRouletteBetRequestDTO, userId: userId);
                roulette.BetList.Add(newBet);
                await _DBConnection.UpdateKey(key: addRouletteBetRequestDTO.RouletteID.ToString(), objectToUpdate: roulette);
                var response = new AddRouletteBetResponseDTO
                {
                    Success = true
                };

                return response;
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }

        private BetDTO MapBetInformation(AddRouletteBetRequestDTO addRouletteBetRequestDTO, Guid userId)
        {
            try
            {
                var newBet = new BetDTO
                {
                    Amount = addRouletteBetRequestDTO.Amount,
                    BetByNumber = addRouletteBetRequestDTO.BetByNumber,
                    Color = addRouletteBetRequestDTO.Color?.Trim().ToUpper(),
                    Number = addRouletteBetRequestDTO.Number,
                    UserID = userId
                };

                return newBet;
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }

        public async Task<List<CloseRouletteResponseDTO>> CloseRoulette(Guid rouletteID)
        {
            try
            {
                int winningNumber = CalculateWinningNumber();
                RouletteDTO roulette = await _DBConnection.GetByKey<RouletteDTO>(key: rouletteID.ToString());
                roulette.IsOpen = false;
                roulette.WinningNumber = winningNumber;
                List<CloseRouletteResponseDTO> winnersList = WinnersList(betList: roulette.BetList, winningNumber: winningNumber);
                await _DBConnection.UpdateKey(key: rouletteID.ToString(), objectToUpdate: roulette);

                return winnersList;
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }

        private int CalculateWinningNumber()
        {
            try
            {
                Random random = new Random();
                int winningNumber = random.Next(0, 36);

                return winningNumber;
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }

        private List<CloseRouletteResponseDTO> WinnersList(List<BetDTO> betList, int winningNumber)
        {
            try
            {
                var winnerColor = winningNumber % 2 == 0 ? "ROJO" : "NEGRO";
                var betsListForWinnerNumber = betList
                                                .Where(b => b.Number == winningNumber)
                                                .Select(b => new CloseRouletteResponseDTO
                                                {
                                                    UserID = b.UserID,
                                                    Amount = b.Amount,
                                                    EarnedValue = b.Amount * 5
                                                })
                                                .ToList();
                var betsListForWinnerColor = betList
                                                .Where(b => !b.BetByNumber && b.Color == winnerColor)
                                                .Select(b => new CloseRouletteResponseDTO
                                                {
                                                    UserID = b.UserID,
                                                    Amount = b.Amount,
                                                    EarnedValue = b.Amount * (decimal)1.8
                                                })
                                                .ToList();
                var betsListForWinnerAll = betsListForWinnerNumber
                                            .Union(betsListForWinnerColor)
                                            .ToList();

                return betsListForWinnerAll;
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }

        public async Task<List<RouletteResponseDTO>> RoulettesList()
        {
            try
            {
                List<RouletteDTO> roulettesList = await _DBConnection.GetListAll();
                List<RouletteResponseDTO> roulettesListMap = MapRoulettesList(roulettesList: roulettesList);

                return roulettesListMap;
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }

        private List<RouletteResponseDTO> MapRoulettesList(List<RouletteDTO> roulettesList)
        {
            try
            {
                var roulettesListMap = roulettesList
                                        .Select(r => new RouletteResponseDTO
                                        {
                                            ID = r.ID,
                                            Status = GetStatusRoulette(isOpen: r.IsOpen)
                                        })
                                        .ToList();

                return roulettesListMap;
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }

        private string GetStatusRoulette(bool isOpen)
        {
            return isOpen ? "Abierta" : "Cerrada";
        }
    }
}
