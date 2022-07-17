using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Misc;
using Fsm_Generator.DataObjects;


namespace Fsm_Generator.Parser
{
    public class PumlGrammarVisitor<Result> : PumlGrammarBaseVisitor<Result>
    {
        /// <summary>
        /// Parsed model attributes
        /// </summary>
        public ModelData Data { get; set; }

        EventDto? _currentEvent;
        StateDto _currentState;
        TransitionDto? _currentTransition;
        private int _timeout = 0;
        private String _eventAction = "";
        List<DescriptionFragment> _comments = new List<DescriptionFragment>();

        public PumlGrammarVisitor()
        {
            Data = new ModelData();
            _currentState = new StateDto();
        }

        // Turn diagram name into the namespace
        [CLSCompliant(false)]
        public override Result VisitDiagramName([NotNull] PumlGrammar.DiagramNameContext context)
        {
            Data.DiagramName = context.DIAGRAM_NAME().GetText();
            return base.VisitDiagramName(context);
        }

        public override Result VisitLine([NotNull] PumlGrammar.LineContext context)
        {
            // Reset context dependent attributes after line processed
            Result res = base.VisitLine(context);

            // Clear the comment if this line wasn't a comment.
            if (context.comment() == null)
            {
                _comments = new List<DescriptionFragment>();
            }

            return res;
        }

        /*
         * Transitions
         */

        public override Result VisitStateTransition([NotNull] PumlGrammar.StateTransitionContext context)
        {
            _currentTransition = new TransitionDto
            {
                EndStateName = context.toState().GetText().Replace("[*]", "End"),
                StartStateName = context.fromState().GetText().Replace("[*]", "Start")
            };
            _currentTransition.EventName = "EVENT_";
            _currentTransition.Description.AddRange(_comments);

            // Event action is optional
            if (context.eventAction() == null)
            {
                // The transition fires when the source state's Do loop is done
                _currentTransition.EventName += "DONE";
            }
            else
            {
                if (context.eventAction().@event().EVENT_NAME() != null)
                {
                    _currentTransition.EventName += context.eventAction().@event().EVENT_NAME().GetText().ToUpper();
                }
                if (context.eventAction().@event().EVENT_TIMEOUT() != null)
                {
                    _currentTransition.EventName += context.eventAction().@event().EVENT_TIMEOUT().GetText().ToUpper();
                }
            }

            Data.AddMergeTransition(_currentTransition);

            return base.VisitStateTransition(context);
        }

        /*
         * States
         */

        public override Result VisitState([NotNull] PumlGrammar.StateContext context)
        {


            _currentState = new StateDto
            {
                Timeout = _timeout
            };
            if (context.startState() != null)
            {
                _currentState.StateName = "Start";
            }
            else
            {
                _currentState.StateName = context.IDENTIFIER().GetText();
            }
            _currentState.Description.AddRange(_comments);
            _comments = new List<DescriptionFragment>();
            _currentState = Data.AddMergeState(_currentState);

            Result result = base.VisitState(context);

            return result;
        }

        public override Result VisitStateInternalEvent([NotNull] PumlGrammar.StateInternalEventContext context)
        {
            Result res = base.VisitStateInternalEvent(context);

            if (_currentEvent?.EventName == "EVENT_ONENTRY")
            {
                _currentState.OnEntryDescription.AddRange(_comments);
            }
            else if (_currentEvent?.EventName == "EVENT_ONEXIT")
            {
                _currentState.OnExitDescription.AddRange(_comments);
            }


            return res;
        }

        public override Result VisitAction([NotNull] PumlGrammar.ActionContext context)
        {
            Result res = base.VisitAction(context);

            if (_currentEvent?.EventName == "EVENT_ONENTRY")
            {
                _currentState.OnEntryAction = context.ACTION_TEXT()?.GetText();
            }
            else if (_currentEvent?.EventName == "EVENT_ONEXIT")
            {
                _currentState.OnExitAction = context.ACTION_TEXT()?.GetText();
            }


            return res;
        }

        public override Result VisitEventAction([NotNull] PumlGrammar.EventActionContext context)
        {
            _eventAction = context.action().GetText();


            return base.VisitEventAction(context);
        }



        public override Result VisitEvent([NotNull] PumlGrammar.EventContext context)
        {
            _currentEvent = new EventDto
            {
                EventName = "EVENT_"
            };

            if (context.EVENT_NAME() != null)
            {
                _currentEvent.EventName += context.EVENT_NAME().GetText().ToUpper();
            }
            if (context.EVENT_TIMEOUT() != null)
            {
                _currentEvent.EventName += context.EVENT_TIMEOUT().GetText().ToUpper();
            }
            Data.AddMergeEvent(_currentEvent);

            return base.VisitEvent(context);
        }

        public override Result VisitFromState([NotNull] PumlGrammar.FromStateContext context)
        {

            return base.VisitFromState(context);
        }

        public override Result VisitGuard([NotNull] PumlGrammar.GuardContext context)
        {

            return base.VisitGuard(context);
        }

        public override Result VisitTimeoutGuard([NotNull] PumlGrammar.TimeoutGuardContext context)
        {
            String timeoutStr = context.guard().TIMEOUT_MS().GetText();
            int.TryParse(timeoutStr, out _timeout);

            return base.VisitTimeoutGuard(context);
        }

        public override Result VisitComment([NotNull] PumlGrammar.CommentContext context)
        {
            // Remove the comment token (') from the text
            String comment = context.COMMENT_LINE().GetText().Replace("' ", "");

            DescriptionFragment df = new DescriptionFragment
            {
                DescriptionLine = comment
            };
            _comments.Add(df);

            return base.VisitComment(context);
        }
    }


}
