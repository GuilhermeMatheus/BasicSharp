using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.Compiler.Lexer
{
    public class SlidingText : IDisposable
    {
        /// <summary>
        /// Char inválido, nunca deve ser consumido nas etapas de compilação
        /// </summary>
        public const char INVALID_CHAR = char.MaxValue;

        readonly StreamReader stream;
        Queue<char> peekBuffer;

        /// <summary>
        /// Representa a linha relativa à InitialPosition
        /// </summary>
        public int Line { get; private set; }
        /// <summary>
        /// Representa a coluna relativa à InitialPosition
        /// </summary>
        public int Column { get; private set; }
        public int InitialPosition { get; private set; }
        public int Offset { get; private set; }

        /// <summary>
        /// Inicializa uma nova instância de BasicSharp.Compiler.Lexer.SlidingText
        /// com a fonte de dados.
        /// </summary>
        /// <param name="sourceStream">Stream para fonte de dados</param>
        public SlidingText(Stream sourceStream)
        {
            if (sourceStream == null)
                throw new ArgumentNullException("sourceStream");

            this.stream = new StreamReader(sourceStream);
            this.peekBuffer = new Queue<char>();
            this.Reset(0);
        }

        public void Reset(int position)
        {
            this.stream.BaseStream.Position = this.InitialPosition = position;
            this.Offset = this.Column = 0;
            this.Line = 1;

            this.stream.DiscardBufferedData();
            this.peekBuffer.Clear();
        }

        public char Next()
        {
            Offset++;
            if (peekBuffer.Count > 0)
                return updateLineColumn(peekBuffer.Dequeue());

            var c = stream.Read();
            return updateLineColumn(charOrInvalidCharFrom(c));
        }

        //Este método deve ser chamado apenas em Next(), pois ele é o único que deve atualizar a posição do leitor
        char updateLineColumn(char c)
        {
            if (c == INVALID_CHAR)
                return c;

            if (c == '\r')
            {
                this.Line++;
                this.Column = 0;
            }
            else
                this.Column++;

            return c;
        }

        public char Next(int jumps)
        {
            var c = INVALID_CHAR;
            for (int i = 0; i < jumps; i++)
            {
                c = Next();
                if (c == INVALID_CHAR)
                    return c;
            }

            return c;
        }

        public char Peek()
        {
            if (peekBuffer.Count > 0)
                return peekBuffer.Peek();

            var peek = stream.Peek();
            return charOrInvalidCharFrom(peek);
        }

        public char Peek(int jumps)
        {
            if (jumps == 0)
                return Peek();

            if (peekBuffer.Count >= jumps+1)
                return peekBuffer.ElementAt(jumps);

            int peek = INVALID_CHAR;
            for (int i = 0; i < jumps+1; i++)
            {
                peek = stream.Read();
                if (peek < 0)
                    return INVALID_CHAR;

                peekBuffer.Enqueue((char)peek);
            }

            return peekBuffer.ElementAt(jumps);
        }

        public int JumpUntil(char c)
        {
            var current = INVALID_CHAR;
            var jumps = 0;
            
            while((current = Next()) != c)
            {
                if (current == INVALID_CHAR)
                    return -1;

                jumps++;
            }

            return jumps;
        }

        /// <summary>
        /// Check if the next characters in the stream match the given string and, case true, advances
        /// the current position
        /// </summary>
        /// <param name="desired">The given string to check</param>
        /// <returns>returns True if the next characters in the stream match the given string.
        /// Otherwise, returns false</returns>
        public bool AdvanceIfMatches(string desired)
        {
            if (desired.Length == 1)
                return AdvanceIfMatches(desired[0]);

            var result = Matches(desired);
            
            if (result)
                Next(desired.Length);
            
            return result;
        }

        /// <summary>
        /// Check if the next characters in the stream match the given char and, case true, advances
        /// the current position
        /// </summary>
        /// <param name="desired">The given char to check</param>
        /// <returns>returns True if the next char in the stream match the given char.
        /// Otherwise, returns false</returns>
        public bool AdvanceIfMatches(char desired)
        {
            if (Peek() == desired)
            {
                Next();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the next characters in the stream match the given string
        /// </summary>
        /// <param name="desired">The given string to check</param>
        /// <returns>returns True if the next characters in the stream match the given string.
        /// Otherwise, returns false</returns>
        public bool Matches(string desired)
        {
            for (int i = 0; i < desired.Length; i++)
                if (Peek(i) != desired[i])
                    return false;

            return true;
        }

        public void Dispose()
        {
            stream.Dispose();
        }

        char charOrInvalidCharFrom(int c)
        {
            if (c < 0)
                return INVALID_CHAR;

            return (char)c;
        }
    }
}