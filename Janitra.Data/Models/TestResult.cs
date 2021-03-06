﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Janitra.Data.Models
{
	public class TestResult
	{
		[Key]
		public int TestResultId { get; set; }

		public int CitraBuildId { get; set; }
		public int JanitraBotId { get; set; }
		public int TestDefinitionId { get; set; }

		public DateTimeOffset ReportedAt { get; set; }

		[Required, Url]
		public string LogUrl { get; set; }

		[Required, Url]
		public string ScreenshotTopUrl { get; set; }

		[Required, Url]
		public string ScreenshotBottomUrl { get; set; }

		public ExecutionResult ExecutionResult { get; set; }

		public AccuracyStatus AccuracyStatus { get; set; }

		public TimeSpan TimeTaken { get; set; }

		//Navigation Fields

		public CitraBuild CitraBuild { get; set; }
		public JanitraBot JanitraBot { get; set; }
		public TestDefinition TestDefinition { get; set; }
	}
}