import { useQuery } from "@tanstack/react-query";
import { getBriefing } from "../api/briefingApi";

/**
 * @param {"saved" | "daily" | "weekly"} type
 */
export function useBriefing(type) {
    return useQuery({
        queryKey: ["briefing", type],
        queryFn: () => getBriefing(type),
    });
}