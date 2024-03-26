namespace AssemblyAI.Realtime.CLI
{
    internal class Utterance
    {
        public string Value { get; set; }
    }
    internal class TextBuilder
    {
        private List<Utterance> utterances = new List<Utterance>();
        private Utterance? currentUtterance;
        public delegate void TextChangedEvent(TextBuilder sender, string text);
        public event TextChangedEvent OnTextChanged;

        public TextBuilder(RealtimeProcessor processor)
        {
            processor.OnAudioResponse += this.Processor_OnAudioResponse;
        }

        private void Processor_OnAudioResponse(RealtimeProcessor sender, Responses.AudioResponse e)
        {
            switch (e.Type)
            {
                case Enums.ResponseType.PartialTranscript:
                    if (currentUtterance == null)
                    {
                        currentUtterance = new Utterance();
                        utterances.Add(currentUtterance);
                    }

                    currentUtterance.Value = e.Text;
                    break;

                case Enums.ResponseType.FinalTranscript:
                    if (currentUtterance == null)
                    {
                        currentUtterance = new Utterance();
                        utterances.Add(currentUtterance);
                    }

                    currentUtterance.Value = e.Text;
                    currentUtterance = null;
                    break;
            }

            Task.Run(() => OnTextChanged?.Invoke(this, this.ToString()));
        }

        public string[] GetUtterances() => utterances.Select(e => e.Value).ToArray();

        public override string ToString()
        {
            return string.Join(" ", utterances.Select(e => e.Value).ToArray());
        }
    }
}
