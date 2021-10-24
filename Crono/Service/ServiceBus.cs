
using Crono.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crono.Service
{
    /// <summary>
    /// Classe per inviare o ricevere messaggi dal bus
    /// </summary>
    public static class ServiceBus
    {
        public static void RaiseCommessaChange(CommessaDto c)
        {
            Messenger.Default.Send<CommessaDto>(c, "CommessaChange");
        }

        public static void SubscribeToCommessaChange(ViewModelBase vm, Action<CommessaDto> callback)
        {
            Messenger.Default.Register<CommessaDto>(vm, "CommessaChange", callback);
        }

        public static void RaiseDateShiftChange(DateTime oldStartDate)
        {
            Messenger.Default.Send<DateTime>(oldStartDate , "DateChange");
        }

        public static void SubscribeToDateShiftChange(object vm, Action<DateTime> callback)
        {
            Messenger.Default.Register<DateTime>(vm, "DateChange", callback);
        }

        public static void RaiseDateEndChange(DateTime newEnd)
        {
            Messenger.Default.Send<DateTime>(newEnd, "DateEndChange");
        }

        public static void SubscribeToDateEndChange(object vm, Action<DateTime> callback)
        {
            Messenger.Default.Register<DateTime>(vm, "DateEndChange", callback);
        }

        public static void RaiseDateStartChange(int difference)
        {
            Messenger.Default.Send<int>(difference, "DateStartChange");
        }

        public static void SubscribeToDateStartChange(object vm, Action<int> callback)
        {
            Messenger.Default.Register<int>(vm, "DateStartChange", callback);
        }

        public static void SubscribeToOpenModal(ViewModelBase vm, Action<TaskDetailsDto> callback)
        {
            Messenger.Default.Register<TaskDetailsDto>(vm, "OpenModal", callback);
        }

        public static void RaiseOpenModal(TaskDetailsDto task)
        {
            Messenger.Default.Send<TaskDetailsDto>(task, "OpenModal");
        }

        public static void SubscribeToCloseModal(ViewModelBase vm, Action<CronoTask[]> callback)
        {
            Messenger.Default.Register<CronoTask[]>(vm, "CloseModal", callback);
        }

        public static void RaiseCloseModal(CronoTask[] task)
        {
            Messenger.Default.Send<CronoTask[]>(task, "CloseModal");
        }

        public static void SubscribeToChangeContent(ViewModelBase vm, Action<object> callback)
        {
            Messenger.Default.Register(vm, "ChangeContent", callback);
        }

        public static void RaiseChangeContent()
        {
            Messenger.Default.Send<object>(null,"ChangeContent");
        }

        public static void SubscribeToResizeHeightEvent(ViewModelBase vm, Action<int> callback)
        {
            Messenger.Default.Register<int>(vm, "ResizeHeightEvent", callback);
        }

        public static void RaiseResizeHeightEvent(int size)
        {
            Messenger.Default.Send< int>(size, "ResizeHeightEvent");
        }

        public static void SubscribeToResizeWidthEvent(ViewModelBase vm, Action<int> callback)
        {
            Messenger.Default.Register<int>(vm, "ResizeWidthEvent", callback);
        }

        public static void RaiseResizeWidthEvent(int size)
        {
            Messenger.Default.Send<int>(size, "ResizeWidthEvent");
        }
    }
}
