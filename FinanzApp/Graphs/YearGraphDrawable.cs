using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Graphics;
using FinanzApp.Data;

namespace FinanzApp.Graphs;

public class YearGraphDrawable : IDrawable
{
    public IList<FinanceEntry> Entries { get; set; } = new List<FinanceEntry>();

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Entries.Count == 0)
            return;

        var months = new List<DateTime>();
        for (int year = 2020; year <= 2030; year++)
            for (int month = 1; month <= 12; month++)
                months.Add(new DateTime(year, month, 1));

        var incomes = new decimal[months.Count];
        var expenses = new decimal[months.Count];
        var balances = new decimal[months.Count];

        decimal running = 0;
        decimal maxBalance = 0;

        for (int i = 0; i < months.Count; i++)
        {
            var dt = months[i];
            var inc = Entries.Where(e => e.Datum.Year == dt.Year && e.Datum.Month == dt.Month && e.Betrag > 0)
                              .Sum(e => e.Betrag);
            var exp = Entries.Where(e => e.Datum.Year == dt.Year && e.Datum.Month == dt.Month && e.Betrag < 0)
                              .Sum(e => e.Betrag);
            running += inc + exp;
            incomes[i] = inc;
            expenses[i] = -exp; // positive value
            balances[i] = running;
            if (running > maxBalance)
                maxBalance = running;
        }

        if (maxBalance == 0)
            maxBalance = 1;

        float width = dirtyRect.Width;
        float height = dirtyRect.Height;
        float stepX = width / (months.Count - 1);

        float prevIncY = height - (float)((double)incomes[0] / (double)maxBalance * height);
        float prevExpY = height - (float)((double)expenses[0] / (double)maxBalance * height);
        float prevBalY = height - (float)((double)balances[0] / (double)maxBalance * height);
        PointF prevInc = new PointF(0, prevIncY);
        PointF prevExp = new PointF(0, prevExpY);
        PointF prevBal = new PointF(0, prevBalY);

        for (int i = 1; i < months.Count; i++)
        {
            float x = i * stepX;
            float yInc = height - (float)((double)incomes[i] / (double)maxBalance * height);
            float yExp = height - (float)((double)expenses[i] / (double)maxBalance * height);
            float yBal = height - (float)((double)balances[i] / (double)maxBalance * height);

            canvas.StrokeColor = Colors.Green;
            canvas.DrawLine(prevInc.X, prevInc.Y, x, yInc);
            canvas.StrokeColor = Colors.Red;
            canvas.DrawLine(prevExp.X, prevExp.Y, x, yExp);
            canvas.StrokeColor = Colors.White;
            canvas.DrawLine(prevBal.X, prevBal.Y, x, yBal);

            prevInc = new PointF(x, yInc);
            prevExp = new PointF(x, yExp);
            prevBal = new PointF(x, yBal);
        }

        // draw axes labels for quarters
        canvas.FontColor = Colors.White;
        for (int i = 0; i < months.Count; i += 3)
        {
            float x = i * stepX;
            var dt = months[i];
            canvas.DrawString($"{dt.Year} Q{(dt.Month - 1) / 3 + 1}", x, height + 2, HorizontalAlignment.Left);
        }
    }
}
