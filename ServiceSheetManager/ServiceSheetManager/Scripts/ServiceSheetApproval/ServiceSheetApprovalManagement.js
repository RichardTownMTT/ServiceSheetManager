$(document).ready(function () {
    CallUpdateTravelOnly = function () {
        var sum = 0;
        $('.totalTravelTime').each(function () {
            sum += parseFloat(this.value);
        });
        $('.JobTotalTravelTime').val(sum);
    }

    CallUpdateOnsiteTime = function () {
        var onsiteSum = 0;
        $('.totalOnsiteTime').each(function () {
            onsiteSum += parseFloat(this.value);
        });
        $('.JobTotalOnsiteTime').val(onsiteSum);
    }

    CallUpdateTravelAndOnsiteTime = function () {
        CallUpdateTravelOnly();
        CallUpdateOnsiteTime();
    }

    CallUpdateDailyAllowanceCount = function () {
        var dailyAllowanceSum = 0;
        $('.DailyAllowance').each(function () {
            dailyAllowanceSum += parseInt(this.value);
        });
        $('.JobTotalDailyAllowances').val(dailyAllowanceSum);
    }

    CallUpdateOvernightAllowanceCount = function () {
        var overnightSum = 0;
        $('.OvernightAllowance').each(function () {
            overnightSum += parseInt(this.value);
        })
        $(".JobTotalOvernightAllowances").val(overnightSum);
    }

    CallUpdateMileage = function () {
        var mileageSum = 0;
        $(".Mileage").each(function () {
            mileageSum += parseInt(this.value);
        })
        $(".JobTotalMileage").val(mileageSum);
    }

    CallUpdateTimeSheetOrder = function () {

        $('#ServiceDayPanel .serviceDayContainer').sort(sortAscending).appendTo("#ServiceDayPanel");

        $(".serviceDayContainer").each(function () {
            var day = $(this).find(".DtReport").val();
            $(this).find(".panelDateHeaderText").text(day);
        });
    }

    function sortAscending(a, b) {
        var date1 = $(a).find(".DtReport").val();
        var date2 = $(b).find(".DtReport").val();

        return date1 > date2 ? 1 : -1;
    }


});