using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1_Маршрутная_перестановка
{
    public class DefaultPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public struct Cell
    {
        public char Value { get; set; }
        public bool Filled { get; set; }
    }

    public class Crypto : DefaultPropertyChanged
    {
        private Cell[,] _permLattice;
        private Cell[,] _permLattice180;
        private Cell[,] _permLattice180Mirror;
        private Cell[,] _permLatticeMirror;

        private List<Cell[,]> _encodingQueue;
        private string _result;

        public Cell[,] PermLattice
        {
            get => _permLattice;
            set
            {
                _permLattice = value;
                OnPropertyChanged();
            }
        }

        public Cell[,] PermLattice180
        {
            get => _permLattice180;
            set
            {
                _permLattice180 = value;
                OnPropertyChanged();
            }
        }

        public Cell[,] PermLattice180Mirror
        {
            get => _permLattice180Mirror;
            set
            {
                _permLattice180Mirror = value;
                OnPropertyChanged();
            }
        }

        public Cell[,] PermLatticeMirror
        {
            get => _permLatticeMirror;
            set
            {
                _permLatticeMirror = value;
                OnPropertyChanged();
            }
        }

        public string Result
        {
            get => _result;
            set
            {
                _result = value; 
                OnPropertyChanged();
            }
        }

        public void Encrypt(string input)
        {
            SearchMultipliers(input.Length, out var row, out var col);
            InitiateArrays(row, col);
            ComputeMarkedCells(row, col);
            ComputeRotatedArrays(row, col);
            ProccesResult(input, row, col);
            GesResult(out var output);
            Result = output;

            OnPropertyChanged(nameof(PermLattice));
            OnPropertyChanged(nameof(PermLattice180));
            OnPropertyChanged(nameof(PermLatticeMirror));
            OnPropertyChanged(nameof(PermLattice180Mirror));
        }

        private void GesResult(out string output)
        {
            output = "";
            foreach (var val in _encodingQueue.Last())
            {
                output += val.Value;
            }
        }

        private void ProccesResult(string input, int row, int col)
        {
            Cell[,] buf = null;
            using (var enumerator = input.GetEnumerator())
            {
                enumerator.Reset();
                foreach (var arr in _encodingQueue)
                {
                    if (buf != null) MergeArrays(arr, buf);
                    for (int i = 0; i < row; i++)
                    {
                        for (int j = 0; j < col; j++)
                        {
                            if (arr[i, j].Filled)
                            {
                                if (!enumerator.MoveNext()) break;
                                arr[i, j].Value = enumerator.Current;
                            }
                        }
                    }
                    buf = arr;
                }
            }
        }

        private void MergeArrays(Cell[,] arr1, Cell[,] arr2)
        {
            for (int i = 0; i < arr1.GetLength(0); i++)
            {
                for (int j = 0; j < arr1.GetLength(1); j++)
                {
                    arr1[i, j].Value = arr2[i, j].Value;
                }
            }
        }


        private void ComputeRotatedArrays(int row, int col)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    PermLattice180[row - 1 - i, j] = PermLattice[i, j];
                    PermLatticeMirror[i, col - 1 - j] = PermLattice[i, j];
                    PermLattice180Mirror[row - 1 - i, col - 1 - j] = PermLattice[i, j];
                }
            }
        }

        private void InitiateArrays(int row, int col)
        {
            PermLattice = new Cell[row, col];
            PermLattice180 = new Cell[row, col];
            PermLattice180Mirror = new Cell[row, col];
            PermLatticeMirror = new Cell[row, col];

            _encodingQueue = new List<Cell[,]> { PermLattice, PermLattice180, PermLattice180Mirror, PermLatticeMirror };

            InititateArray(PermLattice);
            InititateArray(PermLattice180);
            InititateArray(PermLattice180Mirror);
            InititateArray(PermLatticeMirror);
        }

        private void InititateArray(Cell[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i,j] = new Cell();
                }
            }
        }

        private void ComputeMarkedCells(int row, int col)
        {
            var cellsToMark = row / 2 * col / 2;
            int i = 0, j = 0;
            var rnd = new Random();
            var sub_col = col / 2;
            var sub_row = row / 2;
            foreach (var value in Enumerable.Range(0,cellsToMark))
            {
                switch (rnd.Next(0, 4))
                {
                    case 0:
                        i = value / sub_col;
                        j = value % sub_col;
                        break;
                    case 1:
                        i = row - 1 - value / sub_col;
                        j = value % sub_col;
                        break;
                    case 2:
                        i = value / sub_col;
                        j = col - 1 - value % sub_col;
                        break;
                    case 3:
                        i = row - 1 - value / sub_col;
                        j = col - 1 - value % sub_col;
                        break;
                    default:
                        i = 0;
                        j = 0;
                        break;
                }
                PermLattice[i, j].Filled = true;
            }
        }

        private void SearchMultipliers(int product, out int row, out int col)
        {
            if (product < 4) product = 4;
            if (product % 2 != 0) product++;
            row = col = 0;

            while (col == 0)
            {
                int j = product / 2;
                for (int i = 2; i <= j; i += 2)
                {
                    j = product / i;
                    if (j % 2 != 0 || product % i != 0 || i > j) continue;
                    row = i;
                    col = j;
                }
                product += 2;
            }
        }
    }
}
